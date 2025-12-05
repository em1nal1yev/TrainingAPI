using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.Helper
{
    public interface IIdValidationService
    {
        Task ValidateIdsAsync<T>(
        IEnumerable<Guid>? ids,
        Func<IEnumerable<Guid>, Task<List<T>>> fetchFunc,
        string errorMessage);

    }
}
