﻿using System.ComponentModel.DataAnnotations;

namespace UyelikSistemi.ViewModels;

public class SignInViewModel
{
    [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
    [EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
    [Display(Name = "Email:")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Şifre Alanı boş bırakılamaz")]
    [Display(Name = "Şifre:")]
    public string? Password { get; set; }


    public bool RememberMe { get; set; }
}
