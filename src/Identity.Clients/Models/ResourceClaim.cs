// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Models
{
    public class ResourceClaim
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string Type { get; set; }
    }

    public class NewResourceClaim
    {
        public int ResourceId { get; set; }
        public string Type { get; set; }
    }

    public class ChangedResourceClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}