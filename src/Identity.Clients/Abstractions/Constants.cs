// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Abstractions
{
    // public enum SubjectTypes
    // {
    //     Global = 0,
    //     Ppid = 1
    // }

    // public enum AccessTokenType
    // {
    //     Jwt = 0,
    //     Reference = 1
    // }

    // public enum TokenUsage
    // {
    //     ReUse = 0,
    //     OneTimeOnly = 1
    // }

    // public enum TokenExpiration
    // {
    //     Sliding = 0,
    //     Absolute = 1
    // }

    public enum ResourceType
    {
        None,
        Identity,
        Api,
        Grant,
        ApiScope
    }

    public enum ClientUriType
    {
        ClientUri,
        LogoUri,
        RedirectUri,
        PostLogoutRedirectUri,
        CorsUri,
        EventReferenceUri,
        FrontChannelLogoutUri,
        BackChannelLogoutUri,
        EventRegistrationHandlerUri
    }

    [System.Flags]
    public enum ClientFlag
    {
        Published = 0x02,
        RequireConsent = 0x04,
        AllowRememberConsent = 0x08,
        AllowAccessTokensViaBrowser = 0x10,
        AllowOfflineAccess = 0x20,
        AlwaysSendClientClaims = 0x40,
        AlwaysIncludeUserClaimsInIdToken = 0x80,
        UpdateAccessTokenClaimsOnRefresh = 0x100,
        IncludeJwtId = 0x200,
        EnableLocalLogin = 0x400,
        BackChannelLogoutSessionRequired = 0x800,
        FrontChannelLogoutSessionRequired = 0x1000,
        RequireClientSecret = 0x2000,
        RequirePkce = 0x4000,
        AllowPlainTextPkce = 0x8000,
        UseSlidingRefresh = 0x10000,
        UseOneTimeRefreshTokens = 0x20000,
    }

    // public static class ProtocolTypes
    // {
    //     public const string OpenIdConnect = "oidc";
    //     public const string WsFederation = "wsfed";
    //     public const string Saml2p = "saml2p";
    // }

    // public static class TokenTypes
    // {
    //     public const string IdentityToken = "id_token";
    //     public const string AccessToken = "access_token";
    // }

    public static class SecretTypes
        {
            public const string SharedSecret = "SharedSecret";
            public const string X509CertificateThumbprint = "X509Thumbprint";
            public const string X509CertificateName = "X509Name";
            public const string X509CertificateBase64 = "X509CertificateBase64";
        }

}
