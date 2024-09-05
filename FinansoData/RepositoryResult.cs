using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData
{
    public enum ErrorType
    {
        None,
        NotFound,
        Unauthorized,
        ValidationError,
        ServerError,
        MaxGroupsLimitReached,
        NoUserFound
    }

    public class RepositoryResult<T>
    {
        public T Value { get; }
        public bool IsSuccess { get; }
        public string Error { get; }
        public ErrorType ErrorType { get; set; }

        private RepositoryResult(T value, bool isSuccess, string error, ErrorType errorType)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        public static RepositoryResult<T> Success(T value) => new RepositoryResult<T>(value, true, null, ErrorType.None);
        public static RepositoryResult<T> Failure(string error, ErrorType errorType) => new RepositoryResult<T>(default, false, error, errorType);
    }
}
