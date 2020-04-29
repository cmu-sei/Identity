// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Collections.Generic;
using Identity.Clients.Models;

namespace IdentityServer.Models
{
    public class HomeViewModel
    {
        public string Username { get; set; }
        public bool IsPrivileged { get; set; }
        public bool MSIE { get; set; }
        public string UiHost { get; set; }
        public List<ClientSummary> Apps { get; set; }
    }

    public class PublishedClient
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
