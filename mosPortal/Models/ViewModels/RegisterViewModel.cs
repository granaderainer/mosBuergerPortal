﻿using System;
using System.ComponentModel.DataAnnotations;

namespace mosPortal.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Vorname", Prompt = "Vorname")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Nachname", Prompt = "Nachname")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail", Prompt = "example@web.de")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Benutzername", Prompt = "Benutzername")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Geburtsort", Prompt = "Geburtsort")]
        public string Birthplace { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Geburtstag")]
        public DateTime Birthday { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Das {0} muss min. {2} und max. {1} Zeichen lang sein.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).+$", ErrorMessage =
            "Passwort muss aus Großbuchstaben, Kleinbuchstaben, sowie Zahlen und Sonderzeichen bestehen.")]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort", Prompt = "Passwort")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Die beiden Passwörter stimmen nicht überein!")]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen", Prompt = "Passwort")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Land", Prompt = "Land")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Stadt", Prompt = "Stadt")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Straße", Prompt = "Straße")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Hausnummer", Prompt = "Hausnummer")]
        public string Number { get; set; }

        [Required]
        //[StringLength(5, ErrorMessage = "ungültiges Format")]
        [Display(Name = "Postleitzahl", Prompt = "12345")]
        public int ZipCode { get; set; }

        [Display(Name = "Registrierungsschlüssel", Prompt = "Registrierungsschlüssel")]
        [StringLength(11, ErrorMessage = "Der Registrierungsschlüssel ist genau 11 Zeichen lang.", MinimumLength = 11)]
        public string Registerkey { get; set; }
    }
}