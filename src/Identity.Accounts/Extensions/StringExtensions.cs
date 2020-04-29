// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace Identity.Accounts.Extensions
{
    public static class StringExtensions
    {
        public static string ToHash(this string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                return BitConverter.ToString(sha1
                    .ComputeHash(Encoding.UTF8.GetBytes(input.ToLower())))
                    .Replace("-", "")
                    .ToLower();
            }
        }

        public static string ToSha256(this string input)
        {
            using (SHA256 alg = SHA256.Create())
            {
                return BitConverter.ToString(alg
                    .ComputeHash(Encoding.UTF8.GetBytes(input.ToLower())))
                    .Replace("-", "")
                    .ToLower();
            }
        }

        public static bool HasValue(this string s)
        {
            return !String.IsNullOrWhiteSpace(s);
        }

        public static string Before(this string s, char separator)
        {
            int x = s.IndexOf(separator);
            return (x >= 0) ? s.Substring(0, x) : s;
        }

        public static string After(this string s, char separator)
        {
            int x = s.IndexOf(separator);
            return (x >= 0) ? s.Substring(x + 1) : "";
        }

        public static string ToDisplay(this Enum e)
        {
            return e.ToString().Replace("_", " ");
        }

        public static string Extract(this string s, string re)
        {
            Match match = Regex.Match(s, re);
            return match.Groups[match.Groups.Count-1].Value;
        }

        public static bool IsEmailAddress(this string s)
        {
            return Regex.IsMatch(s, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }

        public static string ToTitle(this string s)
        {
            char[] result = s.ToCharArray();
            bool cap = true;
            bool lcap = true;
            for (int i = 0; i < s.Length; i++)
            {
                result[i] = (cap) ? Char.ToUpper(result[i]) : Char.ToLower(result[i]);
                if (result[i] == '\'')
                {
                    cap = lcap; // maintain cap state
                }
                else
                {
                    lcap = cap; // capture last state
                    cap = !Char.IsLetter(result[i]); // update state
                    lcap |= cap; //override state if word separator)
                }
            }
            return new String(result);
        }

        public static string ToAccountSlug(this string s)
        {
            var result = new StringBuilder();
            foreach (char c in s.ToCharArray())
            {
                if (Char.IsLetter(c)) result.Append(Char.ToLower(c));
                if (c == '.' || c == ' ') result.Append('.');
                if (c == '-') result.Append('-');
            }
            return result.ToString();
        }

        public static string UriName(this string s)
        {
            return Path.GetFileNameWithoutExtension(s);
        }
    }
}
