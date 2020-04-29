// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace Identity.Accounts.Models
{
    public class SearchModel
    {
        public string Term { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Since { get; set; }
        public string[] Filter { get; set; } = new string[] {};
    }
}
