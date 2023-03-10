﻿using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentiyApp.Web.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
        [Display(Name = "Email :")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Şifre :")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
