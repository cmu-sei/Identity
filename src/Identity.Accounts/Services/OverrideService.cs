// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;
using Identity.Accounts.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Accounts.Services
{
    public class OverrideService : IOverrideService
    {
        public OverrideService(
            IOverrideStore store,
            IMapper mapper
        )
        {
            _store = store;
            Mapper = mapper;
        }

        private readonly IOverrideStore _store;
        private readonly IMapper Mapper;

        public async Task<OverrideCode[]> List()
        {
            return Mapper.Map<OverrideCode[]>( await _store.List().ToArrayAsync());
        }

        public async Task<OverrideCode> Add(NewOverrideCode model)
        {
            var entity = Mapper.Map<Data.OverrideCode>(model);
            entity.WhenCreated = DateTime.UtcNow;
            entity = await _store.Add(entity);
            return Mapper.Map<OverrideCode>(entity);
        }

        public async Task Delete(int id)
        {
            await _store.Delete(id);
        }
    }
}
