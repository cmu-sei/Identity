// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;
using Identity.Accounts.Models;

namespace Identity.Accounts.Services
{
    public class PropertyService : IPropertyService
    {
        public PropertyService(
            IPropertyStore store,
            IMapper mapper
        )
        {
            _store = store;
            Mapper = mapper;
        }

        private readonly IPropertyStore _store;
        private readonly IMapper Mapper;

        public async Task<AccountProperty> Add(NewAccountProperty model)
        {
            var entity = Mapper.Map<Data.AccountProperty>(model);
            entity = await _store.Add(entity);
            return Mapper.Map<AccountProperty>(entity);
        }

        public async Task<AccountProperty> Update(ChangedAccountProperty model)
        {
            var entity = await _store.Load(model.Id, null);
            if (entity == null)
                throw new InvalidOperationException();

            Mapper.Map(model, entity);
            await _store.Update(entity);
            return Mapper.Map<AccountProperty>(entity);
        }

        public async Task Delete(int id)
        {
            await _store.Delete(id);
        }
    }
}
