// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class Client : IEntity, IEntityPrimary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string GlobalId { get; set; }
        public string EnlistCode { get; set; }
        public string PairWiseSubjectSalt { get; set; }
        [Obsolete] public string ProtocolType { get; set; } = "oidc"; // ProtocolTypes.OpenIdConnect;
        public string ClientClaimsPrefix { get; set; } = "client_";
        public ClientFlag Flags { get; set; } = ClientFlag.RequireConsent | ClientFlag.AllowRememberConsent | ClientFlag.AlwaysIncludeUserClaimsInIdToken;
        public int IdentityTokenLifetime { get; set; } = 300;
        public int AccessTokenLifetime { get; set; } = 3600;
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int ConsentLifetime { get; set; } = 2592000;
        [Obsolete] public int RefreshTokenUsage { get; set; } // = TokenUsage.ReUse;
        [Obsolete] public int RefreshTokenExpiration { get; set; } // = TokenExpiration.Absolute;
        [Obsolete] public int AccessTokenType { get; set; } // = AccessTokenType.Jwt;
        public string Grants { get; set; }
        public string Scopes { get; set; }
        public ICollection<ClientSecret> Secrets { get; set; } = new List<ClientSecret>();
        public ICollection<ClientClaim> Claims { get; set; } = new List<ClientClaim>();
        public ICollection<ClientManager> Managers { get; set; } = new List<ClientManager>();
        public ICollection<ClientEvent> Events { get; set; } = new List<ClientEvent>();
        public ICollection<ClientEventHandler> EventHandlers { get; set; } = new List<ClientEventHandler>();
        public virtual ICollection<ClientUri> Urls { get; set; } = new List<ClientUri>();

        [Obsolete]
        public ICollection<ClientResource> Resources { get; set; } = new List<ClientResource>();
    }
}
