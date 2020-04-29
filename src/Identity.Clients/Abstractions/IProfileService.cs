// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Abstractions
{
    public interface IProfileService
    {
        string Id { get; }
        string Name { get; }
        bool IsAdmin { get; }
        bool IsPrivileged { get; }
    }
}
