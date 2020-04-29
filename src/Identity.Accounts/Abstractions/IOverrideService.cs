// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Identity.Accounts.Models;

namespace Identity.Accounts.Abstractions
{
    public interface IOverrideService
    {
        Task<OverrideCode[]> List();
        Task<OverrideCode> Add(NewOverrideCode property);
        Task Delete(int id);
    }
}
