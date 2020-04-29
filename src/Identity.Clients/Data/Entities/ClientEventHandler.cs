// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class ClientEventHandler : IEntity, IEntityClientProperty
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Uri { get; set; }
        public int ClientEventId { get; set; }
        public ClientEvent ClientEvent { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
