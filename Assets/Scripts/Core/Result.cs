namespace OneDay.Core
{
    public interface IResult
    {
        bool IsSuccessful { get; }
        string Error { get; }
    }

    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
    
    public class Result : IResult
    {
        public bool IsSuccessful { get; private set; }
        public string Error { get; private set; }
        
        public static IResult CreateSuccessful() => new Result(null);
        public static IResult CreateError(string error) => new Result(error);

        protected Result(string error)
        {
            IsSuccessful = error == null;
            Error = error;
        }
    }

    public class Result<T> : Result, IResult<T>
    {
        public T Data { get; private set; }
        
        public static IResult<T> CreateSuccessful(T data) => new Result<T>(data);
        public static IResult<T> CreateError(string error) => new Result<T>(error);
        
        protected Result(string error) : base(error)
        { }
        
        protected Result(T data) : base(null) => Data = data;
    }
}