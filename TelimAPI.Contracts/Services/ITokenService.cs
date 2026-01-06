using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Application.Services
{
    public interface ITokenService
    {
        public string CreateAccessToken(User user, IList<string> roles);

        RefreshToken CreateRefreshToken(Guid userId);
    }
}
