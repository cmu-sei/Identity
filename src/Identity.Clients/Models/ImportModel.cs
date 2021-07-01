// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Clients.Models
{
    public class ResourceImport
    {
        public ApiImport[] Apis { get; set; } = new ApiImport[] {};
        public ClientImport[] Clients { get; set; } = new ClientImport[] {};
    }

    public class ApiImport
    {
        public string Name { get; set; }
        public string Scopes { get; set; }
        public string UserClaims { get; set; }
    }

    public class ClientImport
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string GrantType { get; set; }
        public string Scopes { get; set; }
        public string Secret { get; set; }
        public string[] RedirectUrl { get; set; } = new string[] {};
    }
}
