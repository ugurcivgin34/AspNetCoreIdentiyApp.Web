using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentiyApp.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {

        [Required(ErrorMessage = "Role isim alanı boş bırakılamaz.")]
        [Display(Name = "Role ismi :")]
        public string Name { get; set; }
    }
}
