// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Collections.Generic;

namespace Identity.Clients.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Grants { get; set; }
        public string Scopes { get; set; }
        public bool Enabled { get; set; }
        public bool RequirePkce { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }

        // consent behavior
        public bool RequireConsent { get; set; }
        // public bool AllowRememberConsent { get; set; }
        public string ConsentLifetime { get; set; }

        // tokens behavior
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public string IdentityTokenLifetime { get; set; }
        public string AccessTokenLifetime { get; set; }
        public string AuthorizationCodeLifetime { get; set; }

        // refresh behavior
        public bool AllowOfflineAccess { get; set; }
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public bool UseOneTimeRefreshTokens { get; set; }
        public string SlidingRefreshTokenLifetime { get; set; }
        public string AbsoluteRefreshTokenLifetime { get; set; }

        // urls
        public bool Published { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public string FrontChannelLogoutUrl { get; set; }
        public string BackChannelLogoutUrl { get; set; }
        public ICollection<ClientUri> RedirectUrls { get; set; }
        public ICollection<ClientUri> PostLogoutUrls { get; set; }
        public ICollection<ClientUri> CorsUrls { get; set; }

        // client claims
        public bool AlwaysSendClientClaims { get; set; }
        public string ClientClaimsPrefix { get; set; }
        public ICollection<ClientClaim> Claims { get; set; }

        // inert
        public ICollection<ClientSecret> Secrets { get; set; }
        public ICollection<ClientManager> Managers { get; set; }
        // public ICollection<ClientEvent> Events { get; set; }
        // public ICollection<ClientEventHandler> EventHandlers { get; set; }
    }

    public class ClientDetail : Client
    {
        public string PairWiseSubjectSalt { get; set; }

        public new ICollection<ClientSecretDetail> Secrets { get; set; }
    }

    // public class Client
    // {
    //     public int Id { get; set; }
    //     public string Name { get; set; }
    //     public string DisplayName { get; set; }
    //     public string Description { get; set; }
    //     public bool Enabled { get; set; }
    //     public string GlobalId { get; set; }
    //     public string EnlistCode { get; set; }
    //     public ClientFlag Flags { get; set; }
    //     public ICollection<ClientSecret> Secrets { get; set; }
    //     public ICollection<ClientClaim> Claims { get; set; }
    //     public ICollection<ClientManager> Managers { get; set; }
    //     public ICollection<ClientEvent> Events { get; set; }
    //     public ICollection<ClientEventHandler> EventHandlers { get; set; }
    //     public ICollection<ClientUri> Urls { get; set; }
    //     public ICollection<ClientResource> Resources { get; set; }
    // }

    public class NewClient
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

    }

    public class ChangedClientState
    {
        public int ClientId { get; set; }
        public bool Enabled { get; set; }
    }
}
