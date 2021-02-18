// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Linq;
using Identity.Clients.Extensions;
using Identity.Clients.Models;

namespace Identity.Clients.Mappers
{
    public class ResourceMapper : AutoMapper.Profile
    {
        public ResourceMapper()
        {
            CreateMap<string, string>().ConvertUsing(str => (str is string) ? str.Trim() : null);

            CreateMap<Data.Resource, Resource>()
                .ForMember(d => d.DisplayName, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.DisplayName) ? s.Name : s.DisplayName))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.UserClaims ?? s.Scopes));
            CreateMap<Data.Resource, ResourceDetail>()
                .ForMember(d => d.DisplayName, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.DisplayName) ? s.Name : s.DisplayName))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.UserClaims ?? s.Scopes));
            CreateMap<Resource, Data.Resource>()
                .ForMember(d => d.Managers, opt => opt.Ignore())
                .ForMember(d => d.Secrets, opt => opt.Ignore())
                .ForMember(d => d.Scopes, opt => opt.Ignore())
                .ForMember(d => d.UserClaims, opt => opt.Ignore());
            CreateMap<NewResource, Data.Resource>();
            CreateMap<Data.ApiSecret, ApiSecret>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Description));
            CreateMap<Data.ApiSecret, ApiSecretDetail>();
            CreateMap<ApiSecretDetail, ApiSecret>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Description));

                // .ForMember(d => d.Name, opt => opt.MapFrom(s => s.DisplayName.ToKebabCase()));
            // CreateMap<ChangedResource, Data.Resource>()
            //     .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name.ToKebabCase()));

            // CreateMap<NewResourceClaim, Data.ResourceClaim>();
            // CreateMap<ChangedResourceClaim, Data.ResourceClaim>();

            CreateMap<Data.ResourceManager, ResourceManager>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Name));

            CreateMap<Data.PersistedGrant, PersistedGrant>().ReverseMap();
        }
    }
}
