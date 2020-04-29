// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace IdentityServer.Models
{
    public class fUnregisterModel
    {
        public string LastLogin { get; set; }
        public string Location { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public string Certificate { get; set; }
        public string UserAgent { get; set; }
        public string Data { get; set; }
    }
}
