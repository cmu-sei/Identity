// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;

namespace Identity.Accounts.Data
{
    public class AccountProperty : IEntity
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public virtual Account Account { get; set; }
    }
}
