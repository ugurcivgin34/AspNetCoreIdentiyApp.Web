using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentiyApp.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        //Programcs içinde Şifre için bazı kısıtlamalar koyduk fakat ordaki şeyler kısıtlı , bu metod ile her kısıtı kendimiz yapabiliriz.
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {

            var erros = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                erros.Add(new() { Code = "PasswordNoContainUserName", Description = "Şifre alanı kullanıcı adı içermez" });
            }

            if (password.ToLower().StartsWith("1234"))
            {
                erros.Add(new() { Code = "PasswordNoContain1234", Description = "Şifre alanı ardışık sayı içeremez" });
            }
            if (erros.Any()) //Herhangi bir hata varsa
            {
                return Task.FromResult(IdentityResult.Failed(erros.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);

        }
    }
}
