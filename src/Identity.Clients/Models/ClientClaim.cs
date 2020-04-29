// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Clients.Models
{
    public class ClientClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int ClientId { get; set; }
        public bool Deleted { get; set; }
    }

    public class NewClientClaim
    {
        public int ClientId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

    }

}
