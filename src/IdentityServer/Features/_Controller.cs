// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using IdentityServer.Options;
using System.Linq;
using System;

namespace IdentityServer.Features
{
    public class _Controller : Controller
    {
        public _Controller(
            BrandingOptions displayOptions,
            ILogger logger
        )
        {
            Branding = displayOptions;
            Logger = logger;
        }

        protected BrandingOptions Branding { get; }

        protected ILogger Logger { get; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string host = $"{Request.Scheme}://{Request.Host}";
            ViewBag.AppName = Branding.ApplicationName;
            ViewBag.Title = $"{Branding.ApplicationName} | {Branding.Title}";
            ViewBag.UiHost = Branding.UiHost;
            ViewBag.Meta = string.Join('\n', Branding.Meta)
                .Replace("#app", Branding.ApplicationName)
                .Replace("#title", Branding.Title)
                .Replace("#logo", Branding.LogoUrl ?? $"{host}/logo.png")
                .Replace("#url", host);
        }

        protected void Audit(int action, params object[] fields)
        {
            string msg = string.Join(' ', fields.Select(o => o.ToString()).ToArray());

            Logger.LogWarning(action, "{0} {1}", DateTime.UtcNow, msg);
        }
    }
}
