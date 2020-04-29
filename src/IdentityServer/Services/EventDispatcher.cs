// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Identity.Clients.Abstractions;

namespace IdentityServer.Services
{
    public class EventDispatcher : IEventDispatcher
    {
        HttpClient Client { get; }

        public EventDispatcher (
            HttpClient client
        ){
            Client = client;
        }

        public async Task Send(string url, object payload)
        {
            await Client.PostAsync(
                url,
                new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                )
            );
        }
    }
}
