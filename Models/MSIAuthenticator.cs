using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace MSITestWebApp.Models
{
    public class MSIAuthenticator
    {
        public string AuthResponse = "";

        public void ValidateAuthentication ()
        {
            MSITokenProvider provider = new MSITokenProvider();
            String[] accessTokenResponse = provider.GetMSIAuthToken();
            if (null != accessTokenResponse[0])
            {
                AuthResponse = "Access Token received!";
                try
                {
                    using (SqlConnection con = new SqlConnection("Server=<servername>;Database=<database>;"))
                    {
                        con.AccessToken = accessTokenResponse[0];
                        con.Open();
                        using (SqlCommand cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT @@VERSION";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                reader.Read();
                                AuthResponse += "\nConnected to " + reader.GetString(0);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    AuthResponse += "\n\n" + e.Message;
                }
            }else
            {
                AuthResponse = "Access Token failed with error: \n" + accessTokenResponse[1];
            }
        }
    }
    class MSITokenProvider
    {
        public string[] GetMSIAuthToken()
        {
            string MSIEndpoint = Environment.GetEnvironmentVariable("MSI_ENDPOINT");
            string MSISecret = Environment.GetEnvironmentVariable("MSI_SECRET");
            string URL = "";
            string resource = "https://database.windows.net/";
            string[] response = new string[2];

            /*
            * IsAzureApp is used for identifying if the current client application is running in a Virtual Machine
            * (without MSI environment variables) or App Service/Function (with MSI environment variables) as the APIs to
            * be called for acquiring MSI Token are different for both cases.
            */
            bool IsAzureApp = null != MSIEndpoint && !"".Equals(MSIEndpoint) && null != MSISecret && !"".Equals(MSISecret);

            if (IsAzureApp)
            {
                URL = MSIEndpoint + "?api-version=2017-09-01&resource=" + resource;
            }
            else
            {
                URL = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=" + resource;
            }

            try
            {
                // Build request to acquire managed identities for Azure resources token
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "GET";
                if (IsAzureApp)
                {
                    request.Headers["Secret"] = MSISecret;
                }
                else
                {
                    request.Headers["Metadata"] = "true";
                }
                // Call /token endpoint
                HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

                // Pipe response Stream to a StreamReader, and extract access token
                StreamReader streamResponse = new StreamReader(webResponse.GetResponseStream());
                string stringResponse = streamResponse.ReadToEnd();
                if (IsAzureApp)
                {
                    MSIAccessTokenApp tokenObject = JsonConvert.DeserializeObject<MSIAccessTokenApp>(stringResponse);
                    response[0] = tokenObject.access_token;
                }
                else
                {
                    MSIAccessToken tokenObject = JsonConvert.DeserializeObject<MSIAccessToken>(stringResponse);
                    response[0] = tokenObject.access_token;
                }
                response[1] = null;
            }
            catch (Exception e)
            {
                response[0] = null;
                response[1] = e.Message;
            }
            return response;
        }
    }

    class MSIAccessToken
    {
        public string access_token { get; set; }
        public string client_id { get; set; }
        public long expires_in { get; set; }
        public long expires_on { get; set; }
        public long ext_expires_in { get; set; }
        public long not_before { get; set; }
        public string resource { get; set; }
        public string token_type { get; set; }
    }

    class MSIAccessTokenApp
    {
        public string access_token { get; set; }
        public DateTimeOffset expires_on { get; set; }
        public string resource { get; set; }
        public string token_type { get; set; }
    }
}
