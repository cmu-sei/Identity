// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Models
{
    public class ClientManager
    {
        public int Id { get; set; }
        public string SubjectId { get; set; }
        public string Value { get; set; }
        public int ClientId { get; set; }
        public bool Deleted { get; set; }
    }

    public class NewClientManager
    {
        public string SubjectId { get; set; }
        public string Name { get; set; }
        public int ClientId { get; set; }
    }

    public class ChangedClientManager
    {
        public int Id { get; set; }
        public string SubjectId { get; set; }
        public string Name { get; set; }
    }
}
