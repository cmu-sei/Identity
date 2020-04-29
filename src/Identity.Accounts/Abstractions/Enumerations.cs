// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Accounts.Abstractions
{
    public enum AccountTokenType
    {
        Credential,
        Certificate,
        Password,
        TOTP
    }

    public enum AccountStatus
    {
        Enabled,
        Disabled
    }

    public enum AccountRole
    {
        Member,
        Administrator,
        Manager,
        PowerMember
    }
}
