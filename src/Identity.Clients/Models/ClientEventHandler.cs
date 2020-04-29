// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Models
{
    public class ClientEventHandler
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Uri { get; set; }
        public string EmitterName { get; set; }
        public string EmitterType { get; set; }

    }

    public class NewClientEventHandler
    {
        public string Uri { get; set; }
        public int ClientEventId { get; set; }
        public int ClientId { get; set; }
    }

    public class ChangedClientEventHandler
    {
        public int Id { get; set; }
        public string Uri { get; set; }
    }

    public class ClientEventTarget
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
        public string Status { get; set; }
    }

}