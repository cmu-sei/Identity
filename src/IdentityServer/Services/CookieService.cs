// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Text.Json;
using Identity.Accounts.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Services
{
    public class CookieService
    {
        public CookieService(
            IHttpContextAccessor context,
            IDataProtectionProvider dp,
            AccountOptions accountOptions,
            IHostEnvironment env
        ) {
            _context = context;
            _dp = dp.CreateProtector(AppConstants.DataProtectionPurpose);
            _expires = accountOptions.Password.ResetTokenExpirationMinutes;
            _isProduction = env.IsProduction();
        }

        private readonly IHttpContextAccessor _context;
        private readonly IDataProtector _dp;
        private int _expires = 5;
        private bool _isProduction = true;

        public object Load(string key, Type type)
        {
            string value = _context.HttpContext.Request.Cookies[key];

            var r =  String.IsNullOrEmpty(value)
                ? null
                : JsonSerializer.Deserialize(_dp.Unprotect(value), type);
            return r;
        }

        public bool TryLoad(string key, out object result)
        {
            string value = _context.HttpContext.Request.Cookies[key];
            if (string.IsNullOrEmpty(value))
            {
                result = null;
                return false;
            }
            else
            {
                result = JsonSerializer.Deserialize(_dp.Unprotect(value), typeof(object));
                return true;
            }
        }

        /// <summary>
        /// Append cookie to response
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="minutes"></param>
        /// <remarks>
        /// If minutes = 0, default expiration is used
        /// If minutes < 0, no expiration is used
        /// If minutes > 0, minutes expiration is used
        /// </remarks>
        public void Append(string key, object item, int minutes = 0)
        {
            DateTimeOffset? offset = null;

            int expires = minutes != 0
                ? minutes
                : _expires;

            if (expires > 0)
                offset = new DateTimeOffset(DateTime.UtcNow.AddMinutes(expires));

            string value = JsonSerializer.Serialize(item);
            _context.HttpContext.Response.Cookies.Append(
                key,
                _dp.Protect(value),
                new CookieOptions
                {
                    Expires = offset,
                    IsEssential = true,
                    HttpOnly = true,
                    Secure = _isProduction,
                    SameSite = SameSiteMode.Strict
                });
        }

        public void Remove(string key)
        {
            _context.HttpContext.Response.Cookies.Append(
                key,
                "",
                new CookieOptions
                {
                    Expires = new DateTimeOffset(DateTime.MinValue)
                }
            );
        }

        public string EncryptWithNonce(string plaintext)
        {
            return _dp.Protect($"{Guid.NewGuid()}|{DateTime.Now.Ticks}|{plaintext}");
        }

        public string DecryptWithNonceCheck(string ciphertext, int expiry = 5)
        {
            string plaintext = _dp.Unprotect(ciphertext);
            var parts = plaintext.Split('|');
            if (parts.Length > 2 && Int64.TryParse(parts[1], out long ticks))
            {
                DateTime dt = new DateTime(ticks);
                if (dt.CompareTo(DateTime.Now.AddMinutes(-expiry)) > 0)
                {
                    return parts[2];
                }
            }
            return "";
        }
    }
}
