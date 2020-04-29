// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;

namespace Identity.Accounts.Models
{
    public class TotpResult
    {
        public string Code { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Valid { get; set; }
    }
}
