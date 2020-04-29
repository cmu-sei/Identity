// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Identity.Clients.Models;

namespace IdentityServer.Models
{
    public class ConsentModel
    {
        public string Action { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ConsentViewModel : ConsentModel
    {
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }
        public bool RequestOfflineAccess { get; set; }
        public Resource[] IdentityScopes { get; set; }
        public Resource[] ResourceScopes { get; set; }
    }
}
