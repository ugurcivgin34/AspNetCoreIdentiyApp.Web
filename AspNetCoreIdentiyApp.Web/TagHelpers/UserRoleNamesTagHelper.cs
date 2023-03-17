using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System.Text;

namespace AspNetCoreIdentiyApp.Web.TagHelpers
{
    public class UserRoleNamesTagHelper : TagHelper
    {
        public string UserId { get; set; } = null!;

        private readonly UserManager<AppUser> _userManager;

        public UserRoleNamesTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        ////ProcessAsync() yöntemi, TagHelper'ın işlevselliğinin tanımlandığı yerdir. Bu yöntem içinde, UserManager.GetRolesAsync() yöntemiyle kullanıcının rolleri alınır ve userRoles değişkeninde depolanır.
        //Daha sonra, kullanıcının rolleri bir StringBuilder nesnesinde birleştirilir ve HTML etiketleri olarak formatlanır.Her bir rol, bir span etiketi içinde bir badge (rozet) olarak görüntülenir
        //En son olarak, TagHelperOutput.Content.SetHtmlContent() yöntemi kullanılarak birleştirilmiş HTML kodu TagHelper çıkışına aktarılır ve kullanıcının rolleri görüntülenir.
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var user = await _userManager.FindByIdAsync(UserId);

            var userRoles = await _userManager.GetRolesAsync(user!);

            var stringBuilder = new StringBuilder();

            userRoles.ToList().ForEach(x =>
            {
                stringBuilder.Append(@$"<span class='badge
                    bg-secondary mx-1'>{x.ToLower()}</span>");//@koyarak c# da html kodları yazarken alt alta rahatca yazabiliriz
            });

            output.Content.SetHtmlContent(stringBuilder.ToString());

        }
    }
}
