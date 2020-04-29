// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class ClientClaim : IEntity, IEntityClientProperty
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public string Value { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
