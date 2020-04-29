// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace IdentityServer.Models
{
    public class NoticeModel
    {
        public string ReturnUrl { get; set; }
        public string Next { get; set; }
    }

    public class NoticeViewModel : NoticeModel
    {
        public string Text { get; set; }
    }
}
