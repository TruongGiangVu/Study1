namespace GenApi.Shared.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string? message = null)
        : base(ErrorCode.Required, message)
    {
    }
}
