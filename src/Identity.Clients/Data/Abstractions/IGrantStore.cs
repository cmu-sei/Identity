// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Identity.Clients.Data;

namespace Identity.Clients.Data.Abstractions
{
    public interface IGrantStore
    {
        Task<PersistedGrant[]> List(string subjectId);
        Task<PersistedGrant> Load(string key);
        Task Clear(string subjectId, string clientId);
        Task Clear(string subjectId, string clientId, string type);
        Task Delete(string key);
        Task Save(PersistedGrant grant);
    }
}
