// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Clients.Models
{
    public class ApiSecret
    {
        public int Id { get; set; }
        public int ApiResourceId { get; set; }
        public string Description { get; set; }

    }
}
