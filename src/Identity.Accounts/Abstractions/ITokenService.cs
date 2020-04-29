// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts.Abstractions
{
    public interface ITokenService
    {
        object GenerateJwt(string guid, string name);
    }
}
