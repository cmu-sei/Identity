// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace IdentityServer.Options
{
    public class BrandingOptions
    {
        public string ApplicationName { get; set; } = "Identity";
        public string PathBase { get; set; }
        public string LogoUrl { get; set; }
        public string UiHost { get; set; }
        public string Title { get; set; } = "OpenID Connect";
        public string[] Meta { get; set; } = new string[] {
            @"<meta name=""description"" content=""Serving #title identity and authentication for #app applications"">",
            @"<meta property=""og:type"" content=""website"">",
            @"<meta property=""og:title"" content=""#app | #title"">",
            @"<meta property=""og:url"" content=""#url"">",
            @"<meta property=""og:image"" content=""#logo"">",
            @"<meta property=""og:description"" content=""Serving #title identity and authentication for #app applications"">"
        };

        public bool IncludeSwagger { get; set; } = true;
    }
}
