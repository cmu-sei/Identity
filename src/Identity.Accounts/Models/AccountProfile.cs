// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;

namespace Identity.Accounts.Models
{
    public class AccountProfile
    {
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Avatar { get; set; }
        public string Org { get; set; }
        public string Unit { get; set; }
        public string OrgLogo { get; set; }
        public string UnitLogo { get; set; }
        public DateTime UpdatedAt { get; set; }

    }

    public class AlternateAccountProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar_url { get; set; }
    }

    public class AuthProfile
    {
        public string Id { get; set; }
        public string Role { get; set; }
    }
}
