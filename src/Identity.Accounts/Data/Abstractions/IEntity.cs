// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Accounts.Data.Abstractions
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public interface IEntityGlobal
    {
        string GlobalId { get; set; }
    }
}
