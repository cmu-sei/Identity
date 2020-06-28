// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Text;
using Identity.Accounts.Models;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Exceptions;
using Tests.Common;
using Xunit;

namespace Identity.Accounts.Tests
{
    public class AccountManagerTests : TestCore
    {
        [Theory]
        [InlineData("Jeff Matts0n")]
        [InlineData("Tartans@@1")]
        public void PasswordComplexity_Complex_ReturnsTrue(string password)
        {
            using (var test = CreateTestContext())
            {
                var svc = test.GetAccountService();
                Assert.True(svc.IsPasswordComplex(password));
            }
        }

        [Theory]
        [InlineData("tartans@1")]
        [InlineData("tartans@11")]
        public void PasswordComplexity_Basic_ReturnsFalse(string password)
        {
            using (var test = CreateTestContext())
            {
                var svc = test.GetAccountService();
                Assert.False(svc.IsPasswordComplex(password));
            }
        }

        [Fact]
        public void RegisterAndAuthenicateWithCredential()
        {
            Account userAdded = null;
            Account userFound = null;
            Credentials creds = new Credentials
            {
                Username = "test-auth-creds@test.com",
                Password = "~Tartans@1~"
            };
            using (var init = CreateTestContext())
            {
                var svc = init.GetAccountService();
                userAdded = svc.RegisterWithCredentialsAsync(creds).Result;
            }

            using (var test = CreateTestContext())
            {
                var svc = test.GetAccountService();
                userFound = svc.AuthenticateWithCredentialAsync(creds, "127.0.0.1").Result;
            }

            Assert.True(userAdded != null && userFound != null
                && userFound.Id == userAdded.Id);

        }

        [Fact]
        public void HistoryRequirementWorks()
        {
            Account userAdded = null;
            Credentials creds = new Credentials
            {
                Username = "test-pass-history@test.com",
                Password = "~Tartans@1~"
            };
            using (var test = CreateTestContext())
            {
                var svc = test.GetAccountService();
                userAdded = svc.RegisterWithCredentialsAsync(creds).Result;
                Assert.ThrowsAsync<PasswordHistoryException>(async() =>
                    await svc.ChangePasswordAsync(userAdded.GlobalId, new ChangedPassword { Value = creds.Password })
                ).Wait();
            }
        }

        [Fact]
        public void Reset_Account_ReleasesLockout()
        {
            Account userAdded = null;
            Account userFound = null;
            Credentials creds = new Credentials
            {
                Username = "test-reset@test.com",
                Password = "~Tartans@1~"
            };

            using (var init = CreateTestContext())
            {
                var svc = init.GetAccountService();
                userAdded = svc.RegisterWithCredentialsAsync(creds).Result;
                int code = svc.GenerateAccountCodeAsync(creds.Username).Result.Code;
                creds.Code = code.ToString();
                creds.Password = "321ChangeMe!";
            }

            using (var test = CreateTestContext())
            {
                System.Threading.Tasks.Task.Delay(500).Wait();
                var svc = test.GetAccountService();
                userFound = svc.AuthenticateWithResetAsync(creds, "127.0.0.1").Result;
            }

            Assert.True(userAdded != null && userFound != null
                && userFound.Id == userAdded.Id
                && userFound.Status == AccountStatus.Enabled.ToString()
                && String.IsNullOrEmpty(userFound.LockTimeRemaining));
        }

        [Fact]
        public void Auth_Failures_LockAccount()
        {
            Account userAdded = null;
            Account userFound = null;
            Credentials creds = new Credentials
            {
                Username = "test-lock@test.com",
                Password = "~Tartans@1~"
            };

            using (var init = CreateTestContext())
            {
                init.Options.Authentication.LockThreshold = 3;
                var svc = init.GetAccountService();
                userAdded = svc.RegisterWithCredentialsAsync(creds).Result;

                for (int i=0; i < 3; i++)
                {
                    try
                    {
                        creds.Password += i.ToString();
                        svc.AuthenticateWithCredentialAsync(creds, "").Wait();
                    }
                    catch //(System.Exception ex)
                    {
                    }
                }
            }

            using (var test = CreateTestContext())
            {
                var svc = test.GetAccountService();
                userFound = svc.FindAsync(userAdded.Id).Result;
            }

            Assert.True(userAdded != null && userFound != null
                && userFound.Id == userAdded.Id
                && !String.IsNullOrEmpty(userFound.LockTimeRemaining));
        }

        [Fact]
        public void RegistrationAllowedWithOverrideCode()
        {
            using (var test = CreateTestContext())
            {
                var codeSvc = test.GetOverrideService();
                var overrideAdded = codeSvc.Add(new NewOverrideCode {
                    Code = "JamOverrideCode",
                    Description = "For use from unit test application"
                }).Result;

                var svc = test.GetAccountService();

                Credentials creds = new Credentials
                {
                    Username = "jam-override@test.com",
                    Password = "~Tartans@1~",
                    Code = "JamOverrideCode"
                };
                Account userAdded = svc.RegisterWithCredentialsAsync(creds).Result;
                Assert.NotNull(userAdded);
            }
        }

        // [Fact]
        // public void CanAddCertificateToAccount()
        // {
        //     Credentials creds = new Credentials
        //     {
        //         Username = "test-addcert@test.com",
        //         Password = "~Tartans@1~"
        //     };
        //     var certificate = TestCertificates.UserCertificate;

        //     using (var test = CreateTestContext())
        //     {
        //         var svc = test.GetAccountService();
        //         Account userAdded = svc.RegisterWithCredentialsAsync(creds).Result;
        //         int code = svc.GenerateAccountCodeAsync(userAdded.GlobalId).Result;
        //         userAdded = svc.AddAccountCertificate(
        //                 certificate,
        //                 new Credentials { Username = userAdded.GlobalId, Code = code.ToString()}
        //             ).Result;
        //         // userAdded = svc.AddAccountValidatedSubject(certificate.Subject, new Credentials { Username = userAdded.GlobalId, Code = code.ToString()}).Result;

        //         Account authUser = svc.AuthenticateWithValidatedSubjectAsync(certificate.Subject, "test").Result;
        //         Assert.NotNull(authUser);
        //     }
        // }

        // [Fact]
        // public void CanChangeUsername()
        // {
        //     Credentials creds = new Credentials
        //     {
        //         Username = "test-change-username@test.com",
        //         Password = "~Tartans@1~"
        //     };

        //     using (var test = CreateTestContext())
        //     {
        //         var svc = test.GetAccountService();
        //         Account userAdded = svc.RegisterWithCredentialsAsync(creds).Result;

        //         creds.Username = "test-username@test.com";
        //         svc.ChangeAccountAsync(userAdded.Id, creds).Wait();

        //         Account authUser = svc.AuthenticateWithCredentialAsync(creds, "").Result;
        //         Assert.NotNull(authUser);
        //     }
        // }

        [Fact]
        public void CanUpdateProfileProperties()
        {
            Credentials creds = new Credentials
            {
                Username = "test-update-props@test.com",
                Password = "~Tartans@1~"
            };

            using (var test = CreateTestContext())
            {
                var svc = test.GetAccountService();
                Account userAdded = svc.RegisterWithCredentialsAsync(creds).Result;

                var propSvc = test.GetPropertyService();
                var p2 = propSvc.Add(new NewAccountProperty { AccountId = userAdded.Id, Key = "picture", Value = "jam-avatar" }).Result;
                var p3 = propSvc.Add(new NewAccountProperty { AccountId = userAdded.Id, Key = "orgatar", Value = "cert-logo-url" }).Result;
                Account authUser = svc.AuthenticateWithCredentialAsync(creds, "").Result;
                Assert.True(authUser.Properties.Length == 5);

                p2 = propSvc.Update(new ChangedAccountProperty { Id = p2.Id, Value = "jmattson-avatar" }).Result;
                propSvc.Delete(p3.Id).Wait();

                authUser = svc.AuthenticateWithCredentialAsync(creds, "").Result;
                Assert.True(authUser.Properties.Length == 4);
            }
        }

        [Fact]
        public void TestPassGen()
        {
            using (var test = CreateTestContext())
            {
                var svc = test.GetAccountService();
                string pass = "";
                for (int i = 0; i < 10; i++)
                {
                    pass = PassGen(i);
                    // Console.WriteLine($"password: {pass}");
                }
                Assert.True(svc.IsPasswordComplex(pass));
            }
        }

        [Theory]
        [InlineData("test1@sei.cmu.edu")]
        //[InlineData("test1@andrew.cmu.edu")]
        //[InlineData("test1@cmu.edu")]
        [InlineData("test1@cert.org")]
        //[InlineData("cmu.edu@gmail.com")]
        public void DomainPassesValidCheck(string accountName)
        {
            using (var test = CreateTestContext())
            {
                test.Options.Registration.AllowedDomains = "sei.cmu.edu cert.org";
                var svc = test.GetAccountService();
                Assert.True(svc.IsDomainValid(accountName));
            }
        }

        private string PassGen(int seed)
        {
            Random rand = new Random(seed);
            byte[][] pool = new byte[4][];
            pool[0] = Encoding.ASCII.GetBytes("!@#$%^&*()_-+=[{]};:<>|./?");
            pool[1] = Encoding.ASCII.GetBytes("01234567890123456789012345");
            pool[2] = Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz");
            pool[3] = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            byte[] passbytes = new byte[20];

            int i = 0;
            while (i < passbytes.Length)
            {
                passbytes[i++] = pool[rand.Next(4)][rand.Next(26)];
            }
            return Encoding.ASCII.GetString(passbytes);
        }

    }
}
