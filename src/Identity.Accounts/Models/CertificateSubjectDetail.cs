// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Identity.Accounts.Extensions;

namespace Identity.Accounts.Models
{
    public class CertificateSubjectDetail
    {
        public string Subject { get; set; }
        public string DisplayName { get; set; }
        public string CommonName { get; set; }
        public string UserName { get; set; }
        public string ExternalId { get; set; }
        public string DeprecatedExternalId { get; set; }
        public bool IsAffiliate { get; set; }

        // expecting ldap v3 distinguished names. https://tools.ietf.org/html/rfc2253
        // That's what nginx 1.11+ passes
        public CertificateSubjectDetail(string subjectDN)
        {
            if (string.IsNullOrEmpty(subjectDN))
                return;

            var rdns = new List<string>();
            int i = 0, j = 0;
            char[] chars = subjectDN.ToCharArray();
            char last = '_';
            for (i = 0; i < chars.Length; i++)
            {
                if ((chars[i] == ',' || chars[i] == '+') && last != '\\')
                {
                    rdns.Add(subjectDN.Substring(j, i-j));
                    j = i+1;
                }
                last = chars[i];
            }
            rdns.Add(subjectDN.Substring(j));

            this.Subject = subjectDN;
            this.IsAffiliate = Regex.IsMatch(subjectDN, "affiliate|contractor", RegexOptions.IgnoreCase);
            this.ExternalId = rdns.Where(x => x.StartsWith("UID=")).Select(x => x.Substring(4)).FirstOrDefault();
            this.CommonName = rdns.Where(x => x.StartsWith("CN=")).Select(x => x.Substring(3)).FirstOrDefault();
            var nameParts = this.CommonName.Split('.');

            //if no externalid, parse from CN, dod-style
            if (String.IsNullOrEmpty(ExternalId)
                && Int64.TryParse(nameParts.Last(), out long id))
            {
                this.ExternalId = nameParts.Last();
            }

            // if no "UID=", and no '.uid', use the full subject
            if (String.IsNullOrEmpty(ExternalId))
            {
                ExternalId = Subject;
            }

            // ensure ExternalId has an OU/O context.  Some CA's qualify
            // the UID, some assume their number is unique; but we can't
            // rely on that when accepting certs from various orgs.
            var ou = rdns.Where(x => x.StartsWith("OU=")).Select(x => x.Substring(3));
            var o = rdns.Where(x => x.StartsWith("O=")).Select(x => x.Substring(2));
            bool hasContext = false;
            foreach (string s in ou) hasContext |= ExternalId.Contains(s);
            foreach (string s in o) hasContext |= ExternalId.Contains(s);
            if (!hasContext)
            {
                DeprecatedExternalId = ExternalId;
                ExternalId += "." + ou.LastOrDefault() ?? o.LastOrDefault();
            }

            DisplayName = (nameParts.Length > 1) // assume dod-style
                ? $"{nameParts[1]} {nameParts[0]}"
                : CommonName;

            // clean up display name by removing initials and qualifiers
            nameParts = this.DisplayName
                .Split(' ')
                .Where(x => x.Length > 1 && !x.StartsWith("("))
                .ToArray();

            this.DisplayName = String.Join(" ", nameParts).ToTitle();
            this.UserName = this.DisplayName.ToAccountSlug();
        }
    }
}
