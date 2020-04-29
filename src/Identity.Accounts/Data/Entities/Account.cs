// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;

namespace Identity.Accounts.Data
{
    public class Account : IEntity, IEntityGlobal
    {
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public AccountRole Role { get; set; }
        public DateTime WhenCreated { get; set; }
        public DateTime WhenAuthenticated { get; set; }
        public string WhereAuthenticated { get; set; }
        public DateTime WhenLastAuthenticated { get; set; }
        public string WhereLastAuthenticated { get; set; }
        public int AuthenticationFailures { get; set; }
        public DateTime WhenLocked { get; set; }
        public int LockedMinutes { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublic { get; set; }
        public virtual ICollection<AccountToken> Tokens { get; set; } = new List<AccountToken>();
        public virtual ICollection<AccountProperty> Properties { get; set; } = new List<AccountProperty>();
    }
}
