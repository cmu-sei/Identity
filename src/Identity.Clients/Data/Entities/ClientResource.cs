// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class ClientResource: IEntityClientProperty
    {
        public int ClientId { get; set; }
        public int ResourceId { get; set; }
        public Client Client { get; set; }
        public Resource Resource { get; set; }
    }
}
