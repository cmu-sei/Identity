// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Identity.Clients.Data;

namespace Identity.Clients.Data.Abstractions
{
    public interface IWebhookStore : IDataStore<ClientEventHandler>
    {
        Task<Client> LoadClientWithManagers(int id);
        Task<Client> LoadClientWithManagers(string id);
        Task<ClientEventHandler[]> ListEventTargets(int clientId);
        Task<ClientEvent[]> ListEvents();
        Task<string> GetClientEventRegistrationTarget(int id);
    }
}
