using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad sahəsi məcburidir.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Soyad sahəsi məcburidir.")]
        public string Surname { get; set; } = null!;
        [Required(ErrorMessage = "E-poçt sahəsi məcburidir.")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt ünvanı daxil edin.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Şifrə sahəsi məcburidir.")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Mehkeme id sahəsi məcburidir.")]
        public Guid CourtId { get; set; }
        [Required(ErrorMessage = "Department sahəsi məcburidir.")]
        public Guid DepartmentId { get; set; }
    }
}
