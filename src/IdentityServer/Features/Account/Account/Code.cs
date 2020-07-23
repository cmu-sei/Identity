// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    public class CodeModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Code { get; set; }
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
        public string EncodedReturnUrl { get; set; }
    }

    public class CodeViewModel : CodeModel
    {
        public bool CodeSent { get; set; }
    }

    public class CodeState
    {
        public string Token { get; set; }
        public bool Remember { get; set; }
    }
}
