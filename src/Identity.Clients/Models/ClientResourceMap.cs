// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Models
{
    public class ClientResourceMap
    {
        public int ClientId { get; set; }
        public int ResourceId { get; set; }
    }

    public class ClientResource
    {
        public int ClientId { get; set; }
        public Resource Resource { get; set; }

    }
}