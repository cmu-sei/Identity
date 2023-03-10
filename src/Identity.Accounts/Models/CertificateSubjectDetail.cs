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
        // now supporting x500 format, with it's inverse ordering and looser char constraints.
        public CertificateSubjectDetail(string subjectDN)
        {
            if (string.IsNullOrEmpty(subjectDN))
                return;

            char[] delimiters = new char[] {',', '+', ';'};
            var rdns = new List<string>();
            int i = 0, j = 0;
            char[] chars = subjectDN.ToCharArray();
            char last = '_';
            bool quoted = false;
            bool escaped = false;
            bool x500_ordering = subjectDN.IndexOf("O=") < subjectDN.LastIndexOf("OU=");

            for (i = 0; i < chars.Length; i++)
            {
                // close-quote
                if (chars[i] == '"' && quoted)
                    quoted = false;

                // open-quote
                if (chars[i] == '"' && last == '=')
                    quoted = true;

                // escaped
                escaped = chars[i] != '\\' && last == '\\';

                if (!quoted && !escaped && delimiters.Contains(chars[i]))
                {
                    ParseMultiValueRDN(rdns, subjectDN.Substring(j, i-j).Trim());
                    j = i+1;
                }

                last = chars[i];
            }
            // last value
            ParseMultiValueRDN(rdns, subjectDN.Substring(j).Trim());

            this.Subject = subjectDN;
            this.IsAffiliate = Regex.IsMatch(subjectDN, "affiliate|contractor", RegexOptions.IgnoreCase);
            this.ExternalId = rdns.Where(x => x.StartsWith("UID=")).Select(x => x.Substring(4)).FirstOrDefault();
            this.CommonName = rdns.Where(x => x.StartsWith("CN=")).Select(x => x.Substring(3)).FirstOrDefault();
            var nameParts = this.CommonName.Split('.');

            //if no externalid, parse from CN, dod-style
            if (String.IsNullOrEmpty(ExternalId)
                && Int64.TryParse(nameParts.Last(), out _))
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
            foreach (string s in ou.Union(o)) hasContext |= ExternalId.Contains(s);
            // foreach (string s in o) hasContext |= ExternalId.Contains(s);
            if (!hasContext)
            {
                string suffix = x500_ordering
                    ? ou.FirstOrDefault() ?? o.FirstOrDefault()
                    : ou.LastOrDefault() ?? o.LastOrDefault()
                ;
                DeprecatedExternalId = ExternalId;
                ExternalId += "." + suffix;
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

        private void ParseMultiValueRDN(List<string> list, string rdn)
        {
            int e = rdn.IndexOf('=');
            string key = rdn.Substring(0, e);
            string val = rdn.Substring(e+1).Replace("\"", "");
            string[] multi = val.Split('+');
            list.Add($"{key}={multi[0].Trim()}");
            foreach (string p in multi.Skip(1))
                list.Add(p);
        }
    }
}
