// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Security.Cryptography.X509Certificates;

namespace Identity.Accounts.Abstractions
{
    public interface IIssuerService
    {
        bool Validate(X509Certificate2 certificate);
    }
}
