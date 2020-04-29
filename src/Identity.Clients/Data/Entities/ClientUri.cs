// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class ClientUri : IEntity, IEntityClientProperty
    {
        public int Id { get; set; }
        public ClientUriType Type { get; set; }
        public string Value { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
