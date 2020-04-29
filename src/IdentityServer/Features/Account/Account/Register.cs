// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    public class CredentialModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The two passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }

    }

    public class RegisterViewModel: CredentialModel
    {
        public bool AllowRegistration { get; set; }
        public bool RequireCertificate { get; set; }
        public string AllowedDomains { get; set; }
        public string Complexity { get; set; }
        public string CertificateIssuers { get; set; }
    }

    public class ResetViewModel: CredentialModel
    {
        public string Complexity { get; set; }
    }

}
