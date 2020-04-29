// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Models
{
    public class ClientEvent
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
    }

    public class NewClientEvent
    {
        public string Type { get; set; }
        public int ClientId { get; set; }
    }

    public class ChangedClientEvent
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}