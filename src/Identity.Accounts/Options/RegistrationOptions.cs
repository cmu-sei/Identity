// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;

namespace Identity.Accounts.Options
{
    public class RegistrationOptions
    {
        public bool AllowManual { get; set; } = false;
        public string AllowedDomains { get; set; }
        public bool StoreName { get; set; } = true;
        public bool StoreEmail { get; set; } = true;
        public bool AllowMultipleUsernames { get; set; } = false;
    }
}
