// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Abstractions;

namespace Identity.Clients.Tests
{
    public class ProfileService : IProfileService
    {
        public Profile Profile { get; set; }
        public string Id => Profile?.Id;
        public string Name =>  Profile?.Name;
        public bool IsPrivileged => Profile?.IsPrivileged ?? false;
        public bool IsAdmin => Profile?.IsAdmin ?? false;
    }

    public class Profile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPrivileged { get; set; }
    }
}
