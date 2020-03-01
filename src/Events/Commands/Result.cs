namespace Events
{
    public class Result
    {
        public bool IsError { get; set; }
        public string Message { get; set; }

        public Result() {}
        public Result(string message) {}

        public static Result Ok() => new Result();

        public static Result Error(string message) => new Result { IsError = true, Message = message };   
    }
}
