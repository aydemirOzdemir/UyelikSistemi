using System.ComponentModel.DataAnnotations;

namespace UyelikSistemi.ViewModels;

public class SignUpViewModel
{
    [Required(ErrorMessage ="Kullanıcı Alanı boş bırakılamaz")]
    [Display(Name ="Kullanıcı Adı:")]
    public string? UserName { get; set; }




    [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
    [EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
    [Display(Name = "Email:")]
    public string? Email { get; set; }



    [Required(ErrorMessage = "Telefon Alanı boş bırakılamaz")]
    [Display(Name = "Telefon Numarası:")]
    public string? Phone { get; set; }



    [Required(ErrorMessage = "Şifre Alanı boş bırakılamaz")]
    [Display(Name = "Şifre:")]
    public string? Password { get; set; }


    [Compare(nameof(Password),ErrorMessage ="Şifreler aynı değil.")]
    [Required(ErrorMessage = "Şifre Tekrar Alanı boş bırakılamaz")]
    [Display(Name = "Şifre Tekrarı:")]
    public string? PasswordConfirm { get; set; }
}
