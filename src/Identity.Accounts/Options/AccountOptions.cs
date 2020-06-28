// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts.Options
{
    public class AccountOptions
    {
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
        public string AdminGuid { get; set; }
        public string OverrideCode { get; set; }
        public PasswordOptions Password { get; set; } = new PasswordOptions();
        public RegistrationOptions Registration { get; set; } = new RegistrationOptions();
        public AuthenticationOptions Authentication { get; set; } = new AuthenticationOptions();
        public CertValidationOptions CertValidation { get; set; } = new CertValidationOptions();
        public ProfileOptions Profile { get; set; } = new ProfileOptions();
    }
}
