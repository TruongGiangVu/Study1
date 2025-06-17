namespace GenApi.Shared.Exceptions;

public class BaseException : Exception
{
    // public HttpStatusCode StatusCode { get; }
    public ErrorCode ErrCode { get; set; } = ErrorCode.UnKnow;
    public List<string>? Details { get; set; } = null;
    public BaseException(ErrorCode code = ErrorCode.UnKnow, string? message = null, List<string>? details = null)
        : base(CustomErrorMessage(code, message))
    {
        ErrCode = code;
        Details = details;
    }

    private static string? CustomErrorMessage(ErrorCode code = ErrorCode.UnKnow, string? message = null)
    {
        return code.GetErrorMessage(message);
    }
}
