// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Data;

namespace Identity.Clients.Data.Abstractions
{
    public interface IClientStore : IDataStore<Client>
    {
        IQueryable<Client> List(string term);
        Task<Client> Load(string id);
        Task<Client> LoadByEnlistCode(string code);
        Task<bool> CanManage(int clientId, string subjectId);
        Task<bool> CanManage(string clientGlobalId, string subjectId);

    }
}
