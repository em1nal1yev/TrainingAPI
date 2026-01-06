using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Username { get; set; }
        

        [Required(ErrorMessage = "Yeni şifrə tələb olunur.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Şifrələr uyğun gəlmir.")]
        public string ConfirmPassword { get; set; }
    }
}
