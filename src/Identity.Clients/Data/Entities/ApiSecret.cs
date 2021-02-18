// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class ApiSecret : IEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = SecretTypes.SharedSecret;
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }

    }
}