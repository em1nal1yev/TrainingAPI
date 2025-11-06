using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.Auth;

namespace TelimAPI.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterUserAsync(RegisterDto model, string roleName);
        Task<AuthResult> LoginUserAsync(LoginDto model);
        Task SignOutUserAsync();
        Task<bool> IsInRoleAsync(string email, string roleName);
    }
}
