// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Extensions;
using Identity.Clients.Models;
using Microsoft.Extensions.Logging;

namespace Identity.Clients.Services
{
    public class ResourceService
    {
        private readonly IProfileService _profile;
        private readonly IResourceStore _store;
        private readonly ILogger _logger;
        private readonly IMapper Mapper;

        public ResourceService
        (
            IResourceStore store,
            IProfileService profile,
            ILogger<ResourceService> logger,
            IMapper mapper
        ) {
            _logger = logger;
            _store = store;
            _profile = profile;
            Mapper = mapper;
        }

        public async Task<Resource[]> List(SearchModel search)
        {
            var query = (await _store.GetAll()).AsQueryable();

            if (Enum.TryParse<ResourceType>(search.Filter.FirstOrDefault(), true, out ResourceType type))
            {
                query = query.Where(r => r.Type == type);
            }

            if (!_profile.IsPrivileged)
            {
                query = query.Where(r => r.Type == ResourceType.Identity || r.Managers.Any(m => m.SubjectId == _profile.Id));
            }

            if (search.Term.HasValue())
            {
                query = query.Where(
                    r => r.Name.Contains(search.Term, StringComparison.OrdinalIgnoreCase)
                );
            }

            query = query.OrderBy(c => c.Name);

            if (search.Skip > 0)
                query = query.Skip(search.Skip);

            if (search.Take > 0)
                query = query.Take(search.Take);

            return Mapper.Map<Resource[]>(query.ToArray());
        }

        public async Task<Resource[]> LoadAll()
        {
            return Mapper.Map<Resource[]>(await _store.GetAll());
        }

        public async Task<Resource> Load(int id)
        {
            var entity = await _store.Load(id);
            if (entity == null)
                throw new InvalidOperationException();

            if (!await CanManage(id))
                throw new InvalidOperationException();

            return Mapper.Map<Resource>(entity);
        }

        public async Task<Resource> Add(NewResource model)
        {
            if (model.Type != ResourceType.Api && !_profile.IsPrivileged)
                throw new InvalidOperationException();

            var entity = Mapper.Map<Data.Resource>(model);
            entity.Name = model.Name ?? $"new-api-{_profile.Name.ToKebabCase()}-{new Random().Next().ToString("x")}";
            entity.DisplayName = model.DisplayName ?? entity.Name;

            entity.Claims.Add(new Data.ResourceClaim
            {
                Type = ResourceType.Api.ToString(),
            });

            entity.Enabled = _profile.IsPrivileged;

            if (entity.Type == ResourceType.Api && !_profile.IsPrivileged)
                entity.Managers.Add(new Data.ResourceManager { SubjectId = _profile.Id, Name = _profile.Name });

            await _store.Add(entity);

            return Mapper.Map<Resource>(entity);
        }

        public async Task<Resource> Update(Resource model)
        {
            if (! await CanManage(model.Id))
                throw new InvalidOperationException();

            var entity = await _store.Load(model.Id);

            if (entity == null)
                throw new InvalidOperationException();

            bool state = entity.Enabled;

            Mapper.Map(model, entity);

            entity.Enabled = _profile.IsPrivileged
                ? model.Enabled
                : state;

            UpdateManagers(entity, model.Managers);

            try
            {
                await _store.Update(entity);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message.ToLower().Contains("unique"))
                    throw new ResourceNameNotUniqueException();
                else
                    throw new ResourceUpdateException();
            }

            return Mapper.Map<Resource>(entity);
        }

        private void UpdateManagers(Data.Resource entity, IEnumerable<ResourceManager> managers)
        {
            foreach (var manager in managers.Where(s => s.Deleted))
            {
                var target = entity.Managers.SingleOrDefault(s => s.Id == manager.Id);
                if (target != null)
                    entity.Managers.Remove(target);
            }
        }

        public async Task Delete(int id)
        {
            if (! await CanManage(id))
                throw new InvalidOperationException();

            await _store.Delete(id);
        }

        private async Task<bool> CanManage(int id)
        {
            return _profile.IsPrivileged || await _store.CanManage(id, _profile.Id);
        }

        public async Task<string> NewEnlistCode(int id)
        {
            var entity = await _store.Load(id);
            if (!await CanManage(id))
                throw new InvalidOperationException();

            string code = Guid.NewGuid().ToString("N");
            entity.EnlistCode = code;
            await _store.Update(entity);
            return code;
        }

        public async Task Enlist(string code)
        {
            var resource = await _store.LoadByEnlistCode(code);

            if (resource == null)
                return;

            var entity = resource.Managers.Where(m => m.SubjectId == _profile.Id).SingleOrDefault();
            if (entity == null)
            {
                resource.Managers.Add(new Data.ResourceManager
                {
                    SubjectId = _profile.Id,
                    Name = _profile.Name,
                    ResourceId = resource.Id
                });
                await _store.Update(resource);
            };
        }
    }
}
