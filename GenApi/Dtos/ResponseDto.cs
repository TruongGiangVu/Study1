using System.Text.Json.Serialization;

namespace GenApi.Dtos;

/// <summary>
/// wrapper class for responding from api
/// </summary>
public class ResponseDto
{
    [JsonPropertyOrder(1)]
    public bool IsSuccess => ErrCode == ErrorCode.Success;

    [JsonPropertyOrder(2)]
    public string Code => ErrCode.ToErrorCodeString();

    [JsonIgnore]
    private ErrorCode ErrCode { get; set; } = ErrorCode.UnKnow;

    [JsonPropertyOrder(3)]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyOrder(4)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Details { get; set; } = null;

    public ResponseDto(ErrorCode code = ErrorCode.UnKnow, string? message = null, List<string>? details = null)
    {
        SetProperties(code, message, details);
    }

    public void SetProperties(ErrorCode code = ErrorCode.UnKnow, string? message = null, List<string>? details = null)
    {
        ErrCode = code;
        Message = ErrCode.GetErrorMessage(message);
        Details = details;
    }
    public static ResponseDto Fail(string? message = null, List<string>? details = null)
    {
        return Fail(ErrorCode.UnKnow, message, details);
    }
    public static ResponseDto Fail(ErrorCode code, string? message = null, List<string>? details = null)
    {
        return new ResponseDto(code, message, details);
    }

    public string ToLogString()
    {
        return $"Code:{Code}, Message:{Message}{(Details is not null ? $", Details:{Details.ToJsonString()}" : "")}";
    }
}

public class ResponseDto<T> : ResponseDto
{
    [JsonPropertyOrder(5)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Payload { get; set; } = default;

    public ResponseDto(ErrorCode code = ErrorCode.UnKnow,
                    string? message = null,
                    T? payload = default,
                    List<string>? details = null) : base(code, message, details)
    {
        AttachPayload(payload);
    }

    public void AttachPayload(T? payload) => Payload = payload;

    public static ResponseDto<T> Success(T? payload)
    {
        return new ResponseDto<T>(ErrorCode.Success, payload: payload);
    }
}
