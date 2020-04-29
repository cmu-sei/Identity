// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;

namespace Identity.Accounts.Data
{
    public class OverrideCode : IEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime WhenCreated { get; set; }
    }
}
