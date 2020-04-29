// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts.Options
{
    public class CertValidationOptions
    {
        public string IssuerCertificatesPath { get; set; } = "./certs";
        public bool CheckRevocationOnline { get; set; }
        public bool CheckChainRevocation { get; set; }
        public int VerificationTimeoutSeconds { get; set; }
    }
}
