using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Okta.Sdk;
using Okta.Sdk.Configuration;
using OktaDemo.Models;

namespace OktaDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var userClaims = HttpContext.User.Claims;
            var email = userClaims.FirstOrDefault(claim => claim.Type=="email").Value;

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = configuration.GetSection("Okta").GetValue<string>("OktaDomain"),
                Token = accessToken
            });

            var user = await client.Users.GetUserAsync(email);

            return View();
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
