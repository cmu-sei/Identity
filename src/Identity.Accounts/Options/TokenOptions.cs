// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts.Options
{
    public class TokenOptions
    {
        public string Scheme { get; set; } = "app-scheme";
        public string Key { get; set; } = "app-token-secret-key";
        public string Issuer { get; set; } = "app-issuer";
        public string Audience { get; set; } = "app-audience";
        public int ExpirationMinutes { get; set; } = 60;
    }

}
