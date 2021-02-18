// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    public class PasswordModel
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class PasswordViewModel : PasswordModel
    {
        public string Complexity { get; set; }
    }

}
