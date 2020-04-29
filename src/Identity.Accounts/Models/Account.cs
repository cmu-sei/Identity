// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;

namespace Identity.Accounts.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public string Role { get; set; }
        public DateTime WhenCreated { get; set; }
        public DateTime LastLogin { get; set; }
        public string LastIp { get; set; }
        public string LockTimeRemaining { get; set; }
        public int LockedSeconds { get; set; }
        public string Status { get; set; }
        public string Avatar { get; set; }
        public string OrgLogo { get; set; }
        public string UnitLogo { get; set; }
        public AccountProperty[] Properties { get; set; } = new AccountProperty[] { };

    }

    public class AccountCode
    {
        public string GlobalId { get; set; }
        public int Code { get; set; }
    }
}
