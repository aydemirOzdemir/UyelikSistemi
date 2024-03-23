using System.ComponentModel.DataAnnotations;

namespace UyelikSistemi.ViewModels
{
    public class PasswordChangeViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Alanı boş bırakılamaz")]
        [Display(Name = "Eski Şifre:")]
        [MinLength(6,ErrorMessage = "Şifre En Az 6 Karakter olabilir")]
        public string PasswordOld { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni Şifre Alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre:")]
        [MinLength(6, ErrorMessage = "Şifre En Az 6 Karakter olabilir")]
        public string PasswordNew { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni Şifre Onay Alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre Tekrar:")]
        [MinLength(6, ErrorMessage = "Şifre En Az 6 Karakter olabilir")]
        public string PasswordConfirm { get; set; }
    }
}
