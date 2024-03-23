using Microsoft.AspNetCore.Authorization;

namespace UyelikSistemi.Requirements
{   /// <summary>
/// Burada business işlemleri yaptığımız sınıf bu sınıfa gerekli olan parametreler için kullanıyoruz.
/// </summary>
    public class ExchangeExpirementRequirement:IAuthorizationRequirement
    {
    }


    /// <summary>
    /// Bu sınıf bizim asıl business işlemleri yaptığımız handler sınıfımız.
    /// </summary>
    public class ExchangeExpirementRequirementHandler : AuthorizationHandler<ExchangeExpirementRequirement>
    {
        protected override  Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpirementRequirement requirement)
        {
            var hasExchangeExpireClaim = context.User.HasClaim(x=>x.Type=="ExchangeExpireDate");//Bu tipte bir claim var mı yok mu onu kontrol ediyoruz.
            if (!hasExchangeExpireClaim)
            {
                context.Fail();
                return Task.CompletedTask;

            }

            var exchangeExpireClaim = context.User.FindFirst("ExchangeExpireDate");//Burada claimi alıyoruz.

            if(DateTime.Now> Convert.ToDateTime(exchangeExpireClaim.Value))
            {
                context.Fail();
                return Task.CompletedTask;
            }// bu if de şuna baktık eğer dedik bizim son gün olarak belirttiğimiz günden şuan girdiğimiz gün büyükse bizim işlemimize fail verecek ve sayfayı göstermeyecektir.


            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

}
