using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentiyApp.Web.ClaimProvider
{

    //Cookieden alıp claimlere ekliyor
    //Bu sınıf sadece authrorize attribute olan yerlede çalışır
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;
        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        //User.Idendity.Name direk olarak cookie üzerinden kullanıcı ile ilgili name almaya sağlıyor


        // Authentication Middleware aracılığıyla kullanıcının kimlik bilgilerini dönüştürmek için kullanılan metot.
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Gelen principal nesnesinin Identity özelliğindeki kullanıcı bilgileri ClaimsIdentity tipindedir.
            var identityUser = principal.Identity as ClaimsIdentity;

            // _userManager nesnesi kullanarak, kimlik bilgileri alınan kullanıcıya ait bilgiler alınır.
            var currentUser = await _userManager.FindByNameAsync(identityUser!.Name!);

            // Eğer kullanıcının City özelliği boş ise, gelen principal nesnesi geri döndürülür.
            if (String.IsNullOrEmpty(currentUser!.City))
            {
                return principal;
            }

            // Eğer kullanıcının City özelliği dolu ise, bir Claim öğesi oluşturulur ve kullanıcının kimlik bilgilerine eklenir.
            if (principal.HasClaim(x => x.Type != "city"))
            {
                Claim cityClaim = new Claim("city", currentUser.City);

                identityUser.AddClaim(cityClaim);
            }

            // Düzenlenmiş kimlik bilgileriyle birlikte, gelen principal nesnesi geri döndürülür.
            return principal;
        }

    }
}
