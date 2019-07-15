# Sample Console App to test MSI Authentication with .Net Core 2.2

This sample application can be used to test MSI Authentication from an **"Identity"** enabled Azure App Service/Function. The application code acquires Access Token by making `HttpWebRequest` call to target Azure Services, and then creates `SqlConnection` instance with the access token acquired.

Alternatively, for testing MSI Authentication based connectivity with **Azure Virtual Machine** using Console App, please refer to sample app here: [cheenamalhotra/msi-netcore-sample](https://github.com/cheenamalhotra/msi-netcore-sample)

### Packages Referenced:
- Microsoft.Data.SqlClient 1.0.19189.1-Preview
- Newtonsoft.Json 12.0.2

> NOTE: This code to fetch access token should not be used in production environments, please refer [Microsoft Documentation](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/how-to-use-vm-token) to handle HTTP Error scenarios.

## Pre-Requisites
- .Net Core SDK 2.2
- Identity Feature enabled on Azure App Service/Function App where this application will run.
- Access provided to Azure Resource (App Service/Function App) to connect to target Azure database. 
    - Quick resolution: Run this query in Azure Database _(Login with Active Directory account)_:
    
        `CREATE USER [<AZURE_RESOURCE_NAME>] FROM EXTERNAL PROVIDER`

## Steps to run this application:
- Clone the repository
- Provide `Server Name` and `Database` in `Models\MSIAuthenticator.cs`.
- Deploy the app to Azure App Service.
- Below response confirms connectivity established:

    ```bash
    Access Token received!
    Connected to Microsoft SQL Azure (RTM) - 12.0.2000.8 
        Jul  3 2019 10:02:53 
        Copyright (C) 2019 Microsoft Corporation
    ```
