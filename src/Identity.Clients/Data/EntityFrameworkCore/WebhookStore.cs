// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

// using System.Linq;
// using System.Threading.Tasks;
// using Identity.Clients.Abstractions;
// using Identity.Clients.Data.Abstractions;
// using Identity.Clients.Data;
// using Identity.Clients.Data.EntityFrameworkCore.Abstractions;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Caching.Distributed;

// namespace Identity.Clients.Data.EntityFrameworkCore
// {
//     public class WebhookStore : DataStore<ClientEventHandler>, IWebhookStore
//     {
//         public WebhookStore(
//             ClientDbContext dbContext,
//             IDistributedCache cache = null
//         ) : base (dbContext, cache)        {

//         }

//         public override async Task<ClientEventHandler> Load(int id)
//         {
//             return await DbContext.ClientEventHandlers
//                 .Include(h => h.Client)
//                 .Include(h => h.ClientEvent).ThenInclude(e => e.Client)
//                 .Where(h => h.Id == id)
//                 .SingleOrDefaultAsync();
//         }

//         public async Task<Client> LoadClientWithManagers(int id)
//         {
//             return await DbContext.Clients
//                 .Include(c => c.Managers)
//                 .Where(c => c.Id == id)
//                 .FirstOrDefaultAsync();
//         }

//         public async Task<Client> LoadClientWithManagers(string id)
//         {
//             return await DbContext.Clients
//                 .Include(c => c.Managers)
//                 .Where(c => c.Name == id)
//                 .FirstOrDefaultAsync();
//         }

//         public async Task<ClientEventHandler[]> ListEventTargets(int id)
//         {
//             return await DbContext.ClientEventHandlers
//                 .Include(h => h.Client)
//                 .Include(h => h.ClientEvent)
//                 .ThenInclude(e => e.Client)
//                 .Where(h => h.ClientEvent.ClientId == id)
//                 .ToArrayAsync();
//         }

//         public async Task<ClientEvent[]> ListEvents()
//         {
//             return await DbContext.ClientEvents
//                 .Include(e => e.Client)
//                 .ToArrayAsync();
//         }

//         // public override async Task<ClientEventHandler> Add(ClientEventHandler ev)
//         // {
//         //     var entity = await base.Add(ev);
//         //     return await Load(entity.Id);
//         // }

//         public async Task<string> GetClientEventRegistrationTarget(int id)
//         {
//             return await DbContext.ClientUris
//                 .Where(c =>
//                     c.ClientId == id
//                     && c.Type == ClientUriType.EventRegistrationHandlerUri)
//                 .Select(u => u.Value)
//                 .FirstOrDefaultAsync();

//         }
//     }
// }
