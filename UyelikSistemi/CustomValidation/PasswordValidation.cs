using Microsoft.AspNetCore.Identity;
using UyelikSistemi.Models;

namespace UyelikSistemi.CustomValidation
{
    public class PasswordValidation : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {  
            var errors=new List<IdentityError>();
            
            if (password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new IdentityError { Code = "PasswordNoContainsUserName", Description = "Şifre Kullanıcı Adını içeremez." });
            }
            if (password.ToLower().StartsWith("123456789"))
            {
                errors.Add(new IdentityError
                {
                  Code="PasswordNoStartWithDigit",
                 Description="Şifre sayısal bir değerle başlayamaz."
                });
            }

            if (errors.Any())
            {
                Task.FromResult(IdentityResult.Failed());
            }

         return   Task.FromResult(IdentityResult.Success);


        }
    }
}
