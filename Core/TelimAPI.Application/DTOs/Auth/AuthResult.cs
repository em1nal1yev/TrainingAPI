using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Auth
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public string? AccessToken { get; set; } 
        public string? RefreshToken { get; set; }
    }
}
