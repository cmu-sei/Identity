// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Models
{
    public class ResourceManager
    {
        public int Id { get; set; }
        public string SubjectId { get; set; }
        public string Value { get; set; }
        public int ResourceId { get; set; }
        public bool Deleted { get; set; }
    }
}
