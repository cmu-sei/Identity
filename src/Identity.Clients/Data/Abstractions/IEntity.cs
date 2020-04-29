// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Clients.Data.Abstractions
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public interface IEntityClientProperty
    {
        int ClientId { get; set; }
    }

    public interface IEntityResourceProperty
    {
        int ResourceId { get; set; }
    }
}
