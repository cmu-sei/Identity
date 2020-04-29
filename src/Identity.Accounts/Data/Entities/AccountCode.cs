// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using Identity.Accounts.Abstractions;

namespace Identity.Accounts.Data
{
    public class AccountCode
    {
        public string Hash { get; set; }
        public int Code { get; set; }
        public DateTime WhenCreated { get; set; }
    }
}
