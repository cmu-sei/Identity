// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Linq;
using Identity.Clients.Abstractions;
using Identity.Clients.Extensions;
using Identity.Clients.Models;

namespace Identity.Clients.Mappers
{
    public class ClientMapper : AutoMapper.Profile
    {
        public ClientMapper()
        {
            CreateMap<string, string>().ConvertUsing(str => (str is string) ? str.Trim() : null);

            // CreateMap<Data.Client, Client>().ReverseMap();
            CreateMap<Data.ClientEvent, ClientEvent>().ReverseMap();
            CreateMap<Data.ClientUri, ClientUri>().ReverseMap();
                // .ForMember(d => d.TypeName, opt => opt.MapFrom(s => s.Type.ToString()));

            CreateMap<Data.ClientSecret, ClientSecret>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Description));
            CreateMap<Data.ClientSecret, ClientSecretDetail>();
            CreateMap<Data.ClientManager, ClientManager>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Name));
            CreateMap<Data.ClientClaim, ClientClaim>();

            // CreateMap<Data.Client, ClientDetail>();

            CreateMap<Data.ClientEventHandler, ClientEventHandler>()
                .ForMember(d => d.EmitterName, opt => opt.MapFrom(s => s.ClientEvent.Client.Name))
                .ForMember(d => d.EmitterType, opt => opt.MapFrom(s => s.ClientEvent.Type));

            CreateMap<Data.ClientEventHandler, ClientEventTarget>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Client.Name))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Client.DisplayName))
                .ForMember(d => d.Enabled, opt => opt.MapFrom(s => s.Enabled && s.Client.Enabled))
                .ForMember(d => d.Uri, opt => opt.MapFrom(s => s.Uri))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.ClientEvent.Type));

            CreateMap<Data.Client, ClientSummary>()
                .ForMember(d => d.ClientUri, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.ClientUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.LogoUri, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.LogoUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.EventReferenceUri, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.EventReferenceUri).Select(u => u.Value).FirstOrDefault()));

            CreateMap<Data.Client, Client>()
                .ForMember(d => d.RequirePkce, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.RequirePkce)))
                .ForMember(d => d.RequireConsent, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.RequireConsent)))
                .ForMember(d => d.AlwaysIncludeUserClaimsInIdToken, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AlwaysIncludeUserClaimsInIdToken)))
                .ForMember(d => d.AllowOfflineAccess, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AllowOfflineAccess)))
                .ForMember(d => d.AllowAccessTokensViaBrowser, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AllowAccessTokensViaBrowser)))
                .ForMember(d => d.UpdateAccessTokenClaimsOnRefresh, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.UpdateAccessTokenClaimsOnRefresh)))
                .ForMember(d => d.UseOneTimeRefreshTokens, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.UseOneTimeRefreshTokens)))
                .ForMember(d => d.Published, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.Published)))
                .ForMember(d => d.AlwaysSendClientClaims, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AlwaysSendClientClaims)))
                .ForMember(d => d.ConsentLifetime, opt => opt.MapFrom(s => s.ConsentLifetime.ToSimpleTimespan()))
                .ForMember(d => d.IdentityTokenLifetime, opt => opt.MapFrom(s => s.IdentityTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.AccessTokenLifetime, opt => opt.MapFrom(s => s.AccessTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.AuthorizationCodeLifetime, opt => opt.MapFrom(s => s.AuthorizationCodeLifetime.ToSimpleTimespan()))
                .ForMember(d => d.SlidingRefreshTokenLifetime, opt => opt.MapFrom(s => s.SlidingRefreshTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.AbsoluteRefreshTokenLifetime, opt => opt.MapFrom(s => s.AbsoluteRefreshTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.Url, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.ClientUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.LogoUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.FrontChannelLogoutUrl, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.FrontChannelLogoutUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.BackChannelLogoutUrl, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.BackChannelLogoutUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.RedirectUrls, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.RedirectUri)))
                .ForMember(d => d.PostLogoutUrls, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.PostLogoutRedirectUri)))
                .ForMember(d => d.CorsUrls, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.CorsUri)))
                ;
            CreateMap<Data.Client, ClientDetail>()
                .ForMember(d => d.RequirePkce, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.RequirePkce)))
                .ForMember(d => d.RequireConsent, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.RequireConsent)))
                .ForMember(d => d.AlwaysIncludeUserClaimsInIdToken, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AlwaysIncludeUserClaimsInIdToken)))
                .ForMember(d => d.AllowOfflineAccess, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AllowOfflineAccess)))
                .ForMember(d => d.AllowAccessTokensViaBrowser, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AllowAccessTokensViaBrowser)))
                .ForMember(d => d.UpdateAccessTokenClaimsOnRefresh, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.UpdateAccessTokenClaimsOnRefresh)))
                .ForMember(d => d.UseOneTimeRefreshTokens, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.UseOneTimeRefreshTokens)))
                .ForMember(d => d.Published, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.Published)))
                .ForMember(d => d.AlwaysSendClientClaims, opt => opt.MapFrom(s => s.Flags.HasFlag(ClientFlag.AlwaysSendClientClaims)))
                .ForMember(d => d.ConsentLifetime, opt => opt.MapFrom(s => s.ConsentLifetime.ToSimpleTimespan()))
                .ForMember(d => d.IdentityTokenLifetime, opt => opt.MapFrom(s => s.IdentityTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.AccessTokenLifetime, opt => opt.MapFrom(s => s.AccessTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.AuthorizationCodeLifetime, opt => opt.MapFrom(s => s.AuthorizationCodeLifetime.ToSimpleTimespan()))
                .ForMember(d => d.SlidingRefreshTokenLifetime, opt => opt.MapFrom(s => s.SlidingRefreshTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.AbsoluteRefreshTokenLifetime, opt => opt.MapFrom(s => s.AbsoluteRefreshTokenLifetime.ToSimpleTimespan()))
                .ForMember(d => d.Url, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.ClientUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.LogoUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.FrontChannelLogoutUrl, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.FrontChannelLogoutUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.BackChannelLogoutUrl, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.BackChannelLogoutUri).Select(u => u.Value).FirstOrDefault()))
                .ForMember(d => d.RedirectUrls, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.RedirectUri)))
                .ForMember(d => d.PostLogoutUrls, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.PostLogoutRedirectUri)))
                .ForMember(d => d.CorsUrls, opt => opt.MapFrom(s => s.Urls.Where(u => u.Type == ClientUriType.CorsUri)))
                ;
        }
    }
}
