﻿using System.ComponentModel.DataAnnotations;

namespace AHY.Identity.Models
{
    public class UserSignInModel
    {
        [Required(ErrorMessage ="Kullanıcı adı gereklidir.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Parola gereklidir.")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
