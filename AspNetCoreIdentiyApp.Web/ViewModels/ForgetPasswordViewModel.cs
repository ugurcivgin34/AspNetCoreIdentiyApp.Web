using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentiyApp.Web.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage ="email alanı boş bırakılamaz.")]
        [EmailAddress(ErrorMessage ="Email formatı yanlıştır.")]
        [Display(Name ="Email :")]
        public string Email { get; set; }
    }
}
