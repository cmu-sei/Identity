// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Clients.Extensions
{
    public static class StringExtensions
    {

        public static bool HasValue(this string input)
        {
            return !String.IsNullOrWhiteSpace(input);
        }

        public static string ToKebabCase(this string input)
        {
            return input
                .Replace(" ", "-")
                //TODO: strip other symbols
                .ToLower();
        }

        public static string Sha256(this string input)
        {
            if (!input.HasValue())
                return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string ToSimpleTimespan(this int seconds)
        {
            if (seconds == 0)
                return "";

            int d = seconds / 86400;
            int h = seconds / 3600;
            int m = seconds / 60;

            return d >= 1 ? $"{d}d"
            : h >= 1 ? $"{h}h"
            : m >= 1 ? $"{m}m"
            : $"{seconds}s";
        }

        public static int ToSeconds(this string ts)
        {
            if (ts == string.Empty)
                return 0;

            if (Int32.TryParse(ts.Substring(0, ts.Length - 1), out int value))
            {
                char type = ts.Trim().ToCharArray().Last();
                int factor = 1;

                switch (type)
                {
                    case 'd':
                    factor = 86400;
                    break;

                    case 'h':
                    factor = 3600;
                    break;

                    case 'm':
                    factor = 60;
                    break;
                }

                return value * factor;
            }

            throw new ArgumentException("invalid simple-timespan");
        }
    }
}
