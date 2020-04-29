// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Linq;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Extensions;
using Identity.Accounts.Models;
using Identity.Accounts.Options;

namespace Identity.Accounts.Mappers
{
    public class AccountMapper : AutoMapper.Profile
    {
        public AccountMapper()
        {
            CreateMap<string, string>().ConvertUsing(str => (str is string) ? str.Trim() : null);

            CreateMap<Data.Account, Account>()
                .ForMember(d => d.LastLogin, opt => opt.MapFrom(s => s.WhenAuthenticated))
                .ForMember(d => d.LastIp, opt => opt.MapFrom(s => s.WhereAuthenticated))
                .ForMember(d => d.LockedSeconds, opt => opt.MapFrom(s => s.LockDurationSeconds()))
                .ForMember(d => d.LockTimeRemaining, opt => opt.MapFrom(s => s.LockDuration()))
                .AfterMap((s, d, r) => {
                    var opt = r.Items["ProfileOptions"] as ProfileOptions;
                    if (!string.IsNullOrEmpty(opt.ImageServerUrl))
                    {
                        d.Avatar = $"{opt.ImageServerUrl}/{opt.AvatarPath}/{s.GlobalId}";
                        d.OrgLogo = $"{opt.ImageServerUrl}/{opt.OrgLogoPath}/{s.GetProperty(ClaimTypes.OrgLogo)}";
                        d.UnitLogo = $"{opt.ImageServerUrl}/{opt.UnitLogoPath}/{s.GetProperty(ClaimTypes.UnitLogo)}";
                    }
                });

            CreateMap<Data.AccountProperty, AccountProperty>();
            CreateMap<NewAccountProperty, Data.AccountProperty>();
            CreateMap<ChangedAccountProperty, Data.AccountProperty>();

            CreateMap<Data.AccountCode, AccountCode>();

            CreateMap<Data.OverrideCode, OverrideCode>();
            CreateMap<NewOverrideCode, Data.OverrideCode>();

            CreateMap<Data.Account, AccountProfile>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.GetProperty(ClaimTypes.Name)))
                .ForMember(d => d.Biography, opt => opt.MapFrom(s => s.GetProperty(ClaimTypes.Biography)))
                .ForMember(d => d.Org, opt => opt.MapFrom(s => s.GetProperty(ClaimTypes.Org)))
                .ForMember(d => d.Unit, opt => opt.MapFrom(s => s.GetProperty(ClaimTypes.Unit)))
                .AfterMap((s, d, r) => {
                    var opt = r.Items["ProfileOptions"] as ProfileOptions;
                    if (!string.IsNullOrEmpty(opt.ImageServerUrl))
                    {
                        d.Avatar = $"{opt.ImageServerUrl}/{opt.AvatarPath}/{s.GlobalId}";
                        d.OrgLogo = $"{opt.ImageServerUrl}/{opt.OrgLogoPath}/{s.GetProperty(ClaimTypes.OrgLogo)}";
                        d.UnitLogo = $"{opt.ImageServerUrl}/{opt.UnitLogoPath}/{s.GetProperty(ClaimTypes.UnitLogo)}";
                    }
                });

            CreateMap<Data.Account, AlternateAccountProfile>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.GetProperty(ClaimTypes.Name).Replace('.', ' ')))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.GetProperty(ClaimTypes.Username)))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.GetProperty(ClaimTypes.Email)))
            ;

            CreateMap<Data.Account, TokenSummary>()
                .ForMember(d => d.CredentialCount, opt => opt.MapFrom(s => s.Tokens.Count(t => t.Type == AccountTokenType.Credential)))
                .ForMember(d => d.CertificatesCount, opt => opt.MapFrom(s => s.Tokens.Count(t => t.Type == AccountTokenType.Certificate)))
                .ForMember(d => d.HasPassword, opt => opt.MapFrom(s => s.Tokens.Any(t => t.Type == AccountTokenType.Password)))
                .ForMember(d => d.HasTotp, opt => opt.MapFrom(s => s.Tokens.Any(t => t.Type == AccountTokenType.TOTP)))
                .ForMember(d => d.Emails, opt => opt.MapFrom(s => s.Properties.Where(p => p.Key == ClaimTypes.Email).Select(p => p.Value)))
            ;
        }
    }
}
