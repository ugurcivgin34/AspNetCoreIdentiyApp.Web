
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentiyApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Kullanıcı ad alanı boş bırakılamaz.")]
        [DisplayName("Kullanıcı Adı :")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [DisplayName("Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz.")]
        [DisplayName("Telefon :")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [DisplayName("Şifre  :")]
        public string Password { get; set; }

        [Compare(nameof(Password),ErrorMessage ="Şifre aynı değildir")]
        [Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz.")]
        [DisplayName("Şifre Tekrar :")]
        public string PasswordConfirm { get; set; }

    }
}
