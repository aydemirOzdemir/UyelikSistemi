using AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UyelikSistemi.Models;

namespace UyelikSistemi.ClaimProvider
{
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> userManager;

        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity=principal.Identity as ClaimsIdentity;//User claimsleri alırız buradan.
            

            var currentUser =await userManager.FindByNameAsync(identity.Name);
            if (currentUser == null) return principal;
            if (currentUser.City == null) return principal;
            if (principal.HasClaim(x=>x.Type!="city"))
            {
                Claim cityClaim = new Claim("city", currentUser.City);
                identity.AddClaim(cityClaim);
            }
            return principal;
        }
    }
}
