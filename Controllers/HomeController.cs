using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSITestWebApp.Models;

namespace MSITestWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            MSIAuthenticator auth = new MSIAuthenticator();
            auth.ValidateAuthentication();
            return View("index", auth);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
