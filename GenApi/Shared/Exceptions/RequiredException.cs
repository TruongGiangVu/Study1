namespace GenApi.Shared.Exceptions;

public class RequiredException : BaseException
{
    public RequiredException(string? message = null)
        : base(ErrorCode.Required, message)
    {
    }
}
