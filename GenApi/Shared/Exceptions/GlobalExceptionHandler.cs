using GenApi.Dtos;

using Microsoft.AspNetCore.Diagnostics;

using Serilog.Context;

namespace GenApi.Shared.Exceptions;
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string? className = null;
        ResponseDto response;

        if (exception is BaseException baseException) // là BaseException của mình
        {
            httpContext.Response.StatusCode = 200;
            response = ResponseDto.Fail(baseException.ErrCode, baseException.Message, baseException.Details);
        }
        else // nó là Exception của hệ thống không phải exception của mình
        {
            response = ResponseDto.Fail(exception.Message);
        }

        className ??= exception.TargetSite?.DeclaringType?.Name ?? string.Empty;
        using (LogContext.PushProperty(Ct.Common.SourceContext, className))
        {
            string methodName = exception.TargetSite?.Name ?? string.Empty;
            Log.Error("{methodName} handle Exception: {response}",methodName, response.ToJsonString());
            //logger.LogError($"{response.ToLogString()}");
        }
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
