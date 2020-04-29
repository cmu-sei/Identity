// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Clients.Models
{
    public class ApiResourceClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int ApiResourceId { get; set; }
    }
}
