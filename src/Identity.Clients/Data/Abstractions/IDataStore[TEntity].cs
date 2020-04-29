// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Clients.Data.Abstractions
{
    public interface IDataStore<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> List();
        Task<TEntity> Load(int id);
        Task<TEntity> Add(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(int id);
        Task<bool> Exists(int id);

    }
}
