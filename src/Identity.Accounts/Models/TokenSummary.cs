// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts
{
    public class TokenSummary
    {
        public string GlobalId { get; set; }
        public int CredentialCount { get; set; }
        public int CertificatesCount { get; set; }
        public bool HasPassword { get; set; }
        public bool HasTotp { get; set; }
        public string[] Emails { get; set; }
        public string CurrentCertificateSubject { get; set; }
        public string CurrentCertificateIssuer { get; set; }
        public bool CurrentCertificateRegistered { get; set; }
        public string ComplexityRegex { get; set; }
        public string ComplexityRequirement { get; set; }
        public string AllowedDomains { get; set; }
        public bool AllowMultipleCredentials { get; set; }
    }
}
