// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    public class ConfirmModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
        public string Code { get; set; }
        public string Action { get; set; }
        public string State { get; set; }
    }

    public class ConfirmViewModel : ConfirmModel
    {
        public bool CodeSent { get; set; }
        public bool CodeConfirmed { get; set; }
        public string AllowedDomains { get; set; }
        public string CertificateIssuers { get; set; }
    }

    public class ConfirmState
    {
        public string Action { get; set; }
        // public string ReturnUrl { get; set; }
        public string Token { get; set; }
        public bool Confirmed { get; set; }

        public bool IsConfirmed(string token)
        {
            return !String.IsNullOrEmpty(token) && this.Token == token && this.Confirmed;
        }
    }
}
