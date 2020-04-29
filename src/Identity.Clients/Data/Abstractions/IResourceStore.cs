// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Identity.Clients.Data;

namespace Identity.Clients.Data.Abstractions
{
    public interface IResourceStore: IDataStore<Resource>
    {
        Task<Resource[]> GetAll();
        Task<Resource> LoadByEnlistCode(string code);
        Task<bool> CanManage(int resourceId, string subjectId);

    }
}
