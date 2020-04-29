// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;

namespace Identity.Accounts.Models
{
    public class OverrideCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime WhenCreated { get; set; }
    }

    public class NewOverrideCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

}
