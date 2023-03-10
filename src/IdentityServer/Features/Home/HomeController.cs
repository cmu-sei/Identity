// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Accounts.Options;
using IdentityServer.Extensions;
using IdentityServer.Models;
using IdentityServer.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace IdentityServer.Features.Home
{
    [SecurityHeaders]
    public class HomeController : _Controller
    {
        private readonly HomeService _viewSvc;

        public HomeController(
            HomeService clientService,
            BrandingOptions branding,
            AccountOptions authorizationOptions,
            ILogger<HomeController> logger
        ) :base(branding, logger)
        {
            _viewSvc = clientService;
            Options = authorizationOptions;
        }

        AccountOptions Options { get; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _viewSvc.GetHomeView());
        }

        [HttpGet]
        [Authorize(Policy = "PrivilegedUser")]
        public async Task<IActionResult> Test()
        {
            return View("Index", await _viewSvc.GetHomeView());
        }

        [HttpGet]
        public async Task<IActionResult> fUnregister()
        {
            await Task.Delay(0);

            var model = new fUnregisterModel();
            model.Location = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            model.LastLogin = DateTime.UtcNow.AddMinutes(-12345).ToString("s");
            model.UserAgent = Request.Headers[HeaderNames.UserAgent];
            model.Subject = User?.FindFirstValue("sub") ?? Guid.NewGuid().ToString();
            model.Name = User?.Identity?.Name ?? "Ender Wiggin";
            model.Certificate = Request.GetCertificateSubject(
                Options.Authentication.ClientCertHeader,
                Options.Authentication.ClientCertSubjectHeaders
            );
            return View(model);
        }

        public async Task<IActionResult> Error(string errorId)
        {
            return View("Error", await _viewSvc.GetErrorView(errorId));
        }
    }
}
