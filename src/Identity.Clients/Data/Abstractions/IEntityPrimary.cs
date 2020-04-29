// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Data.Abstractions
{
    public interface IEntityPrimary
    {
        string Name { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        bool Enabled { get; set; }
    }

}