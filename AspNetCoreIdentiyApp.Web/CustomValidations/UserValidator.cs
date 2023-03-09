using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentiyApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var erros = new List<IdentityError>();
            var isDigit = int.TryParse(user.UserName[0].ToString(), out _);

            if (isDigit)
            {
                erros.Add(new() { Code = "UserNameContainFirstLetterDigit", Description = "Kullanıcı adının ilk karakteri sayısal bir karakter içeremez" });
            }

            if (erros.Any())
            {
                return Task.FromResult(IdentityResult.Failed(erros.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
