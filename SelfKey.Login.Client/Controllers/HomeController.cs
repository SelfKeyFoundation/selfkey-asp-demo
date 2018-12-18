using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SelfKey.Login.Client.Helpers;
using SelfKey.Login.Client.Models;

namespace SelfKey.Login.Client.Controllers
{
    public class HomeController : Controller
    {
        private const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private const string TenantIdType = "http://schemas.microsoft.com/identity/claims/tenantid";

        private readonly IAuthenticationHelper _authenticationHelper;

        public HomeController(IAuthenticationHelper authenticationHelper)
        {
            _authenticationHelper = authenticationHelper;
        }

        public async Task<IActionResult> Index(string userJson)
        {
            if (User.Identity.IsAuthenticated)
            {
                string token = await _authenticationHelper.GetUserAccessTokenAsync(User.FindFirst(ObjectIdentifierType)?.Value);

                if (String.IsNullOrEmpty(token))
                {
                    return RedirectToAction(nameof(Error), "Home", new { message = "User not found in token cache. The server may have been restarted. Please sign out and sign in again." });
                }

                if (!String.IsNullOrEmpty(userJson))
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    ViewData["Response"] = (await httpClient.PostAsync(
                        AzureAdOptions.Settings.ServiceBaseAddress + "/api/selfkey/verify",
                        new StringContent(userJson, Encoding.UTF8, "application/json"))).IsSuccessStatusCode;

                    ViewData["UserJson"] = userJson;
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            ViewData["Message"] = message;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
