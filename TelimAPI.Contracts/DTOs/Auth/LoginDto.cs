using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-poçt sahəsi məcburidir.")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt ünvanı daxil edin.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Şifrə sahəsi məcburidir.")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
