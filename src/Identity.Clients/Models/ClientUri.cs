// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Abstractions;

namespace Identity.Clients.Models
{
    public class ClientUri
    {
        public int Id { get; set; }
        public ClientUriType Type { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
        public int ClientId { get; set; }
        public bool Deleted { get; set; }
    }

    public class NewClientUri
    {
        public ClientUriType Type { get; set; }
        public string Value { get; set; }
        public int ClientId { get; set; }
    }

    public class ChangedClientUri : ClientUri
    {
    }
}
