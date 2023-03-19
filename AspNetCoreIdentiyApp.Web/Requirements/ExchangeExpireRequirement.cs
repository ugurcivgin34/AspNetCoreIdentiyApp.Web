using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentiyApp.Web.Requirements
{
    /// <summary>
    /// Bir kullanıcının değişim yapabileceği son tarihin geçip geçmediğini kontrol etmek için yetkilendirme gereksinimini temsil eder.
    /// </summary>
    /// 
    //Bu şekilde yaparak program cs'den verilerin buraya gelmesini de sağlamış olduk.Program cs den ne gönderirsek buraya gelecek.Sınıfın propertysi de olsaydı o proerty e data gönderebilecektik.
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {

    }

    /// <summary>
    /// Kullanıcının değişim yapabileceği son tarihin geçip geçmediğini kontrol ederek yetkilendirme gereksinimini işler.
    /// </summary>
    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            // Kullanıcının "ExchangeExpireDate" adlı hak talebi olup olmadığını kontrol eder.
            // Eğer yoksa, yetkilendirme işlemini başarısız olarak sonlandırır.
            if (!context.User.HasClaim(x => x.Type == "ExchangeExpireDate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate")!;

            // "ExchangeExpireDate" adlı hak talebinin değerinin geçerli bir tarih olup olmadığını kontrol eder.
            // Eğer tarih geçmişse, yetkilendirme işlemini başarısız olarak sonlandırır.
            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate.Value))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            // Tarih hala geçmemişse, yetkilendirme işlemini başarılı olarak sonlandırır.
            context.Succeed(requirement);
            return Task.CompletedTask;



            //Bu kodların kullanımı, bir kullanıcının değişim yapabileceği son tarihi kontrol etmek ve buna göre yetkilendirme işlemini gerçekleştirmek için kullanılabilir. Örneğin, bir web uygulamasında kullanıcının belli bir süre içinde sadece belirli sayıda değişim yapabilmesi gerektiği durumlarda bu kodlar kullanılabilir...
        }
    }
}
