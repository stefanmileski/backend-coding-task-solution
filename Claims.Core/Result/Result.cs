namespace Claims.Core.Result
{
    public class Result<T>
    {
        public T? Value { get; }
        public string? Message { get; }
        public ResultType ResultType { get; }

        public bool IsSuccess => ResultType == ResultType.Ok;

        private Result(T value)
        {
            Value = value;
            ResultType = ResultType.Ok;
        }

        private Result(ResultType resultType, string message)
        {
            ResultType = resultType;
            Message = message;
        }

        public static Result<T> Ok(T value) => new(value);
        public static Result<T> NotFound(string message) => new(ResultType.NotFound, message);
        public static Result<T> Invalid(string message) => new(ResultType.Invalid, message);
        public static Result<T> InternalError(string message) => new(ResultType.InternalError, message);
    }
}
