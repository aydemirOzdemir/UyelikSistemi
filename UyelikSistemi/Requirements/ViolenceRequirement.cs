using Microsoft.AspNetCore.Authorization;

namespace UyelikSistemi.Requirements
{
    public class ViolenceRequirement:IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }
    }
    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {
            var hasExchangeExpireClaim = context.User.HasClaim(x => x.Type == "birthdate");//Bu tipte bir claim var mı yok mu onu kontrol ediyoruz.
            if (!hasExchangeExpireClaim)
            {
                context.Fail();
                return Task.CompletedTask;

            }

            var birthDateClaim = context.User.FindFirst("birthdate");//Burada claimi alıyoruz.

            var today=DateTime.Now;
            int yas = today.Year - Convert.ToDateTime(birthDateClaim.Value).Year;

            if (requirement.ThresholdAge>yas)
            {
                context.Fail();
                return Task.CompletedTask;
            }// bu if de şuna baktık eğer dedik bizim son gün olarak belirttiğimiz günden şuan girdiğimiz gün büyükse bizim işlemimize fail verecek ve sayfayı göstermeyecektir.


            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
    }

}
