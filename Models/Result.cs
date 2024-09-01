namespace Models
{
    public class Result<T>
    {
        public T Data { get; private set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; private set; }
        public Exception Exception { get; private set; }

        public static Result<T> Success(T data)
        {
            return new Result<T> { IsSuccess = true, StatusCode = 200, Data = data };
        }

        public static Result<T> Failure(string errorMessage, int statusCode = 400, Exception exception = null)
        {
            return new Result<T> { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode, Exception = exception };
        }
    }
}
