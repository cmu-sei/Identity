// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Identity.Accounts.Abstractions;

namespace Identity.Accounts.Data.Extensions
{
    public static class AccountExtensions
    {
        public static string GeneratePasswordHash(this Account account, string password)
        {
            byte[] target = Encoding.UTF8.GetBytes(password);
            byte[] salt = Guid.Parse(account.GlobalId).ToByteArray();
            int iterations = Math.Abs((salt[0] << 8 | salt[1]) % 10000);
            using (Rfc2898DeriveBytes alg = new Rfc2898DeriveBytes(target, salt, iterations))
            {
                return BitConverter.ToString(alg.GetBytes(32)).Replace("-", "").ToLower();
            }
        }

        public static bool VerifyPassword(this Account account, string password)
        {
            var hash = account.GeneratePasswordHash(password);
            var current = account.CurrentPassword()?.Hash;
            return hash.Equals(current);
        }

        public static AccountToken CurrentPassword(this Account account)
        {
            return account.GetPasswords().FirstOrDefault();
        }

        public static bool IsLocked(this Account account)
        {
            return account.WhenLocked.AddMinutes(account.LockedMinutes) > DateTime.UtcNow;
        }

        public static void Lock(this Account account, int threshold)
        {
            account.AuthenticationFailures += 1;
            if (threshold > 0
                && account.AuthenticationFailures >= threshold)
            {
                account.LockedMinutes = (account.LockedMinutes > 0) ? account.LockedMinutes * 2 : 1;
                account.WhenLocked = DateTime.UtcNow;
                account.AuthenticationFailures = 0;
            }
        }

        public static void Unlock(this Account account)
        {
            account.AuthenticationFailures = 0;
            account.LockedMinutes = 0;
            account.WhenLocked = DateTime.MinValue;
        }

        public static string LockDuration(this Account account)
        {
            return (account.IsLocked())
                ? account.WhenLocked
                    .AddMinutes(account.LockedMinutes)
                    .Subtract(DateTime.UtcNow)
                    .ToString("hh\\:mm\\:ss")
                : "";
        }

        public static int LockDurationSeconds(this Account account)
        {
            return (account.IsLocked())
                ? (int)account.WhenLocked
                    .AddMinutes(account.LockedMinutes)
                    .Subtract(DateTime.UtcNow)
                    .TotalSeconds
                : 0;
        }

        public static bool HasExpiredPassword(this Account account, int age)
        {
            return (age > 0)
                && (DateTime.UtcNow.Subtract(account.CurrentPassword().WhenCreated).TotalDays > age);
        }

        public static bool HasHistoricalPassword(this Account account, string password, int history)
        {
            bool result = false;
            string passwordHash = account.GeneratePasswordHash(password);
            AccountToken[] existingPasswords = account.GetPasswords();
            for (int i = 0; i < Math.Min(existingPasswords.Length, history); i++)
                result |= (existingPasswords[i].Hash == passwordHash);

            return result;
        }

        public static void TrimPasswordHistory(this Account account, int history)
        {
            AccountToken[] existingPasswords = account.GetPasswords();
            for (int i = history; i < existingPasswords.Length; i++)
                account.Tokens.Remove(existingPasswords[i]);
        }

        public static AccountToken[] GetPasswords(this Account account)
        {
            return account.Tokens
                .Where(o => o.Type == AccountTokenType.Password)
                .OrderByDescending(o => o.WhenCreated)
                .ToArray();
        }

        public static void SetAuthenticated(this Account account, string location)
        {
            account.WhenLastAuthenticated = account.WhenAuthenticated;
            account.WhereLastAuthenticated = account.WhereAuthenticated;
            account.WhenAuthenticated = DateTime.UtcNow;
            account.WhereAuthenticated = location;
            account.AuthenticationFailures = 0;
            account.Unlock();
        }

        public static string GetProperty(this Account account, string name)
        {
            return account.Properties.FirstOrDefault(p => p.Key == name)?.Value;
        }

        public static bool IsExpired(this Account account, int idleDays)
        {
            return idleDays > 0
                ? DateTimeOffset.UtcNow > account.WhenLastAuthenticated.AddDays(idleDays)
                : false
            ;
        }
    }
}
