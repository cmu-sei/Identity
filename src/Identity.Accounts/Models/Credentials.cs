// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Linq;
using Identity.Accounts.Extensions;

namespace Identity.Accounts.Models
{
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }

        public string DisplayName
        {
            get
            {
                string[] parts = this.Username
                    .Split('@')[0]
                    .Split('+')[0]
                    .Split('.')
                    .Where(x => x.Length > 1)
                    .ToArray();
                return string.Join(" ", parts).ToTitle();
            }

        }

        public bool IsAffiliate
        {
            get
            {
                return Username.ToLower().Contains(".ctr@");
            }
        }
    }

    public class AccountMergeModel
    {
        public string Code { get; set; }
        public string DefunctGlobalId { get; set; }
        public string ActiveGlobalId { get; set; }
    }

    public class UsernameRegistrationModel
    {
        public string[] Usernames { get; set; }
        public string Password { get; set; }
    }

    public class UsernameRegistration
    {
        /// <summary>
        /// Parses a mailto address into DisplayName and Email Address
        /// </summary>
        /// <param name="value"></param>
        public UsernameRegistration(string value)
        {
            int x = value.IndexOf('<');
            int y = (x > 0) ? value.IndexOf('>', x) : 0;
            Username = value.Substring(x+1, y>0 ? y-x-1 : value.Length - x - 1).Trim();
            IsAffiliate = Username.ToLower().Contains(".ctr@");
            DisplayName = (x > 0)
                ? value.Substring(0, x).Trim().Replace(".", "").Trim()
                : string.Join(" ", Username
                    .Split('@')[0]
                    .Split('+')[0]
                    .Split('.')
                    .Where(x => x.Length > 1)
                    .ToArray()).ToTitle();

        }
        public string Username { get; }
        public string DisplayName { get; }
        public string Message { get; set; }
        public bool IsAffiliate { get; }
    }

    public class ChangedPassword
    {
        public string CurrentPassword { get; set; }
        public string Value { get; set; }
    }
}
