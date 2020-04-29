// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
        public string Provider { get; set; }
    }

    public class LoginViewModel : LoginModel
    {
        public bool AllowRememberLogin { get; set; }
        public bool RequireCertificate { get; set; }
        public int LockedSeconds { get; set; }
        public string Text { get; set; }
        public string CertificateIssuers { get; set; }
        public string[] ExternalSchemes { get; set; }
        public bool MSIE { get; set; }
        public string CertificateSubject { get; set; }
        public string CertificateIssuer { get; set; }
    }

}
