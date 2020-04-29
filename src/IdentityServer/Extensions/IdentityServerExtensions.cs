// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class IdentityServerExtensions
    {
        public static IIdentityServerBuilder AddConfiguredSigningCredential(this IIdentityServerBuilder builder, string certificateFile, string password)
        {
            try
            {
                X509Certificate2 certificate = new X509Certificate2(
                    certificateFile,
                    password,
                    X509KeyStorageFlags.MachineKeySet |
                    X509KeyStorageFlags.PersistKeySet |
                    X509KeyStorageFlags.Exportable);
                    builder.AddSigningCredential(certificate);
                    Console.WriteLine($"Signing credential ADDED [{certificateFile}]");

                    return builder;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Signing credential FAILED [{certificateFile}] : {ex.Message}");
            }

            builder.AddDeveloperSigningCredential();
            return builder;
        }
    }
}
