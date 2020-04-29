// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace IdentityServer.Models
{
    public class LogoutModel
    {
        public string LogoutId { get; set; }
    }

    public class LogoutViewModel : LogoutModel
    {
        public string Referrer { get; set; }
        public bool ShowLogoutPrompt { get; set; }
        public string ClientName { get; set; }
        public string[] ActiveClients { get; set; }
    }
}
