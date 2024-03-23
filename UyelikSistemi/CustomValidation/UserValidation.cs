using Microsoft.AspNetCore.Identity;
using UyelikSistemi.Models;

namespace UyelikSistemi.CustomValidation;

public class UserValidation : IUserValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        var errors=new List<IdentityError>();
        if (char.IsDigit(user.UserName!.FirstOrDefault()))
        {
            errors.Add(new IdentityError()
            {
                Code = "UserNameNoStartWithNumber",
                Description = "Kullanıcı adı sayı karakterleri ile başlamamalıdır."
            });
        }
        if (errors.Any())
        {
            return Task.FromResult(IdentityResult.Failed());
        }


        return Task.FromResult(IdentityResult.Success);
    }
}

