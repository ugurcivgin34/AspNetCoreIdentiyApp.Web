using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCoreIdentiyApp.Web.TagHelpers
{
    public class UserPictureThumbnailTagHelper : TagHelper
    {
        public string? PictureUrl { get; set; }

        //Process() yöntemi, TagHelper sınıfından türetilir ve görüntü etiketi oluşturmak için kullanılır.
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //output değişkeni, TagHelperOutput tipinde bir değişkendir ve img etiketi oluşturulacağını belirtir.
            output.TagName = "img";

            if (string.IsNullOrEmpty(PictureUrl))
            {
                output.Attributes.SetAttribute("src", "/userpictures/default_user_picture.jpg");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/userpictures/{PictureUrl}");
            }
        }
    }
}
