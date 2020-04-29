// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace IdentityServer
{
    public static class LogEventId
    {
        public const int AuthSucceededWithCertRequired = 1000;
        public const int AuthFailedWithCertRequired = 1001;
    }

    public static class LoginMethod
    {
        public const string Creds = "cred";
        public const string Certificate = "cert";
        public const string External = "extn";
        public const string TickOr = "' or";
    }
}
