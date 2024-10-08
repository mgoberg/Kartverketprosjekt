﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace kartverketprosjekt.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "Navn er påkrevd")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig epost")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Emne er påkrevd")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Melding er påkrevd")]
        public string Message { get; set; }
    }
}