// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Identity.Clients.Models;

namespace Identity.Clients.Abstractions
{
    public interface IEventDispatcher
    {
        Task Send(string url, object target);
    }
}