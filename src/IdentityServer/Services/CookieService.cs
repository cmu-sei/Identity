// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Services
{
    public class CookieService
    {
        public CookieService(
            IHttpContextAccessor context,
            IDataProtectionProvider dp
        ) {
            _context = context;
            _dp = dp.CreateProtector(AppConstants.DataProtectionPurpose);
        }

        private readonly IHttpContextAccessor _context;
        private readonly IDataProtector _dp;

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

        public void Append(string key, object item, int minutes = 5)
        {
            string value = JsonSerializer.Serialize(item);
            _context.HttpContext.Response.Cookies.Append(
                key,
                _dp.Protect(value),
                new CookieOptions
                {
                    Expires = new DateTimeOffset(DateTime.UtcNow.AddMinutes(minutes)),
                    IsEssential = true,
                    HttpOnly = true,
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
