namespace GenApi.Shared.Exceptions;

public class ValidationException : BaseException
{
    public ValidationException(string? message = null, List<string>? details = null)
        : base(ErrorCode.Validate, message, details)
    {
    }
}
