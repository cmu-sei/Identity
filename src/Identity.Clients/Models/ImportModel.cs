// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Models
{
    public class ResourceImport
    {
        public string[] Apis { get; set; } = new string[] {};

        public ClientImport[] Clients { get; set; } = new ClientImport[] {};
    }

    public class ClientImport
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string GrantType { get; set; }
        public string Scopes { get; set; }
        public string Secret { get; set; }
        public string RedirectUrl { get; set; }
    }
}
