using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentiyApp.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="email alanı boş bırakılamaz.")]
        [EmailAddress(ErrorMessage ="Email formatı yanlıştır.")]
        [Display(Name ="Email :")]
        public string Email { get; set; }
    }
}
