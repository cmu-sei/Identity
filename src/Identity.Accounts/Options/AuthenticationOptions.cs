// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts.Options
{
    public class AuthenticationOptions
    {
        public bool Require2FA { get; set; } = true;
        public bool RequireNotice { get; set; } = true;
        public string CertificateIssuers { get; set; }
        public string NoticeFile { get; set; } = "wwwroot/html/notice.html";
        public string TermsFile { get; set; } = "wwwroot/html/terms.html";
        public string TroubleFile { get; set; } = "wwwroot/html/trouble.html";
        public int LockThreshold { get; set; }

        public bool AllowCredentialLogin { get; set; } = true;
        public bool AllowAutoLogin { get; set; } = true;
        public bool AllowLocalLogin { get; set; } = true;
        public bool AllowRememberLogin { get; set; } = true;
        public int RememberMeLoginDays { get; set; } = 30;

        public bool ShowLogoutPrompt { get; set; } = true;
        public bool AutomaticRedirectAfterSignOut { get; set; } = false;

        public bool WindowsAuthenticationEnabled { get; set; } = false;
        public readonly string[] WindowsAuthenticationSchemes = new string[] { "Negotiate", "NTLM" };
        public readonly string WindowsAuthenticationDisplayName = "Windows";

        public string SigningCertificate { get; set; }
        public string SigningCertificatePassword { get; set; }

        public string ClientCertHeader { get; set; } = "X-ARR-ClientCert";
        public string ClientCertSubjectHeader { get; set; } = "ssl-client-subject-dn";
        public string ClientCertIssuerHeader { get; set; } = "ssl-client-issuer-dn";
        public string ClientCertVerifyHeader { get; set; } = "ssl-client-verify";
        public string ClientCertSerialHeader { get; set; } = "ssl-client-serial";
    }
}
