// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace IdentityServer.Models
{
    public class LoggedOutViewModel
    {
        public string RedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }
        public bool AutomaticRedirect { get; set; }
        public string LogoutId { get; set; }
    }
}
