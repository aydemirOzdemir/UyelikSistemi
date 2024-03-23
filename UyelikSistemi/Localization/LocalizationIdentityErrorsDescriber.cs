using Microsoft.AspNetCore.Identity;

namespace UyelikSistemi.Localization;

public class LocalizationIdentityErrorsDescriber:IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = "PasswordTooShort",
            Description = $"Şifre {length} değerinden daha kısa olamaz."
        };
    }


}
