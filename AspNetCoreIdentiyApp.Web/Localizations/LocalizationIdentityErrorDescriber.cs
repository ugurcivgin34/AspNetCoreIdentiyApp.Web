using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentiyApp.Web.Localizations
{
    //IdentityErrorDescriber ıdentity servere ait bir classtır.Bu classtan kalıtıma alarak yaptığımız hata mesajlarını türkçeye çevirebiliriz,yani onları ezip kendimiz açıklama yapabiliriz
    public class LocalizationIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DuplicateUserName", Description = $"Bu {userName} daha önce başka bir kullanıcı tarafından alınmıştır." };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new() { Code = "DuplicateUserName", Description = $"Bu {email} daha önce başka bir kullanıcı tarafından alınmıştır." };

        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "DuplicateUserName", Description = $"Şifre en az 6 karakterli olmalıdır." };

        }
    }
}
