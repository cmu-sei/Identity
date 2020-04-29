// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class ResourceManager : IEntity, IEntityResourceProperty
    {
        public int Id { get; set; }
        public string SubjectId { get; set; }
        public string Name { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}
