// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts.Options
{
    public class ProfileOptions
    {
        public bool ForcePublic { get; set; } = false;
        public string Domain { get; set; } = "site.local";
        public string ImageServerUrl { get; set; } = "/javatar";
        public string UnitLogoPath { get; set; } = "u";
        public string OrgLogoPath { get; set; } = "o";
        public string AvatarPath { get; set; } = "p";
        public string DefaultLogo { get; set; } = "default.png";
    }
}
