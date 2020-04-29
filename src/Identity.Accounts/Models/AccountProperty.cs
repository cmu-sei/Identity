// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;

namespace Identity.Accounts.Models
{
    public class AccountProperty
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class NewAccountProperty
    {
        public int AccountId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ChangedAccountProperty
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
