// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using IdentityServer4.Models;

namespace IdentityServer.Extensions
{
    public static class StringExtensions
    {
        public static string SplitCamelCase( this string str )
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string ToHostDomain(this string str)
        {
            var parts = str.Split('.');
            int x = parts.Length - 2;
            if (x > 0)
            {
                str = $"{parts[x]}.{parts[x+1]}";
            }
            return str;
        }

        public static string ReturnUrl(this string url, string returnUrl)
        {
            return String.IsNullOrEmpty(returnUrl)
                ? url
                : $"{url}?ReturnUrl={Uri.EscapeDataString(returnUrl)}";
        }

        public static string AppendQueryString(this string url, string key, string value)
        {
            if (String.IsNullOrEmpty(value))
                return url;

            string item = Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(Uri.UnescapeDataString(value));
            string prefix = url.Contains('?') ? "&" : "?";
            return url + prefix + item;
        }
    }
}
