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
        NoUserFound,
        EmailAlreadyExists,
        RegisterError,
        AssignUserRoleError,
        WrongPassword
    }

    public class RepositoryResult<T>
    {
        /// <summary>
        /// Gets tge value of repository if operation is success
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets a value indicating whether the repository operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error message if the repository operation failed.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets the type of error if the repository operation failed.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        public int? TotalResult { get; }

        public bool IsPageResult { get; set; }

        private RepositoryResult(T value, bool isSuccess, string error, ErrorType errorType, int? totalResult)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
            TotalResult = totalResult;
            IsPageResult = totalResult.HasValue;
        }

        /// <summary>
        /// Creates a success repository result.
        /// </summary>
        public static RepositoryResult<T> Success(T value, int? totalResultCount = null)
        {
 
            return new RepositoryResult<T>(value, true, null, ErrorType.None, totalResultCount);
        } 
            

        /// <summary>
        /// Creates a failed repository result with a default error type.
        /// </summary>
        public static RepositoryResult<T> Failure(string? error, ErrorType errorType) => new RepositoryResult<T>(default, false, error, errorType, null);
    }
}
