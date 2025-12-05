using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Helper;

namespace TelimAPI.Persistence.Helper
{
    public class IdValidationService : IIdValidationService
    {
        public async Task ValidateIdsAsync<T>(IEnumerable<Guid>? ids, Func<IEnumerable<Guid>, Task<List<T>>> fetchFunc, string errorMessage)
        {
            if (ids == null || !ids.Any())
                return;

            var existing = await fetchFunc(ids);

            if (existing.Count != ids.Count())
                throw new Exception(errorMessage);
        }
    }
}
