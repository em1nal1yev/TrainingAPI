using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.Common.Results
{
    public class Result
    {
        public bool Succeeded { get; init; }
        public List<string> Errors { get; init; } = new();

        public static Result Success() => new Result { Succeeded = true };
        public static Result Failure(string error) => new Result { Succeeded = false, Errors = new List<string> { error } };
        public static Result Failure(List<string> errors) => new Result { Succeeded = false, Errors = errors };
    }
}
