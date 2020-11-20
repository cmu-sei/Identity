// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Services;
using IdentityServer.Models;
using IdentityServer.Options;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace IdentityServer.Features.Home
{

    public class HomeService : Mvc.Features.IFeatureService
    {
        private readonly ClientService _clientSvc;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHttpContextAccessor _http;
        private readonly BrandingOptions _branding;

        public HomeService(
            ClientService clientSvc,
            IIdentityServerInteractionService interaction,
            IHttpContextAccessor httpContext,
            BrandingOptions branding
        ){
            _clientSvc = clientSvc;
            _interaction = interaction;
            _http = httpContext;
            _branding = branding;
        }

        public async Task<HomeViewModel> GetHomeView()
        {
            var apps = await _clientSvc.Find(new Identity.Clients.Models.SearchModel { Filter = new string[] {"published"}});
            return new HomeViewModel {
                Username = _http.HttpContext.User.Identity.Name,
                IsPrivileged = _http.HttpContext.User.IsInRole(AppConstants.AdminRole)
                    || _http.HttpContext.User.IsInRole(AppConstants.ManagerRole),
                UiHost = _branding.UiHost,
                MSIE = IsMSIE(_http.HttpContext.Request.Headers[HeaderNames.UserAgent]),
                Apps = apps.ToList()
            };
        }
        public async Task<ErrorViewModel> GetErrorView(string errorId)
        {
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message == null)
                message = new IdentityServer4.Models.ErrorMessage{Error = errorId};

            return new ErrorViewModel
            {
                Error = message
            };
        }

        private bool IsMSIE(string agent)
        {
            if (string.IsNullOrEmpty(agent))
                return false;
            string msiePattern = "msie\\s|trident";
            return Regex.IsMatch(agent, msiePattern, RegexOptions.IgnoreCase);
        }
    }
}
