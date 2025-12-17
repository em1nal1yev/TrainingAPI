using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.Common.Results
{
    public class Result<T> : Result
    {
        public T Data { get; init; }

        public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
        public new static Result<T> Failure(string error) => new() { Succeeded = false, Errors = new List<string> { error } };
    }
}
