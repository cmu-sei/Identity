// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;

namespace Identity.Clients.Models
{
    public class ApiSecret
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool Deleted { get; set; }
    }

    public class ApiSecretDetail
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; }
    }

    public class NewApiSecret
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Plaintext { get; set; }
    }
}
