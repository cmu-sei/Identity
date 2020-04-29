// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Threading.Tasks;
using Identity.Accounts.Data;

namespace Identity.Accounts.Data.Abstractions
{
    public interface IAccountStore : IDataStore<Account>
    {
        Task<bool> IsTokenUnique(string hash);
        Task<Account> Load(int id);
        Task<Account> LoadByGuid(string guid);
        Task<Account> LoadByToken(string hash);
        Task<OverrideCode> GetOverrideCode(string code);
        Task<AccountCode> GetAccountCode(string name);
        Task Save(AccountCode token);
        Task Delete(AccountCode token);
        // Task Add(AccountToken token);
        // Task Delete(AccountToken token);
    }
}
