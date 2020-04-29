// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using Identity.Accounts.Abstractions;

namespace Identity.Accounts.Data
{
    public class AccountToken
    {
        public string Hash { get; set; }
        public DateTime WhenCreated { get; set; }
        public AccountTokenType Type { get; set; }
        public int UserId { get; set; }
        public Account User { get; set; }
    }
}
