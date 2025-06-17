using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace GenApi.Shared;
public static class JsonExtension
{
    /// <summary> To the JSON representation from this object </summary>
    /// <returns> A JSON string </returns>
    public static string ToJsonString(this object? obj)
    {
        try
        {
            return JsonSerializer.Serialize(obj, JsonOptionWriteNormal);

            // return obj is ITuple tuple
            //     ? FormatTupleValues(tuple)
            //     : JsonSerializer.Serialize(obj, JsonOptionWriteNormal);
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(ToJsonString)}: type {obj?.GetType()}:\n exception {ex}");
        }
        return obj?.ToString() ?? $"{nameof(ToJsonString)} ERROR";
    }

    /// <summary> Custom JsonSerializerOption </summary>
    internal readonly static JsonSerializerOptions JsonOptionWriteNormal = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), // cho phép convert chữ tiếng việt
    };

    // <summary> Hàm convert tuple thành json string </summary>
    // private static string FormatTupleValues(ITuple tuple)
    // {
    //     object[]? tupleValues = new object[tuple.Length];
    //     for (int i = 0; i < tuple.Length; i++)
    //     {
    //         var tupleElement = tuple[i];
    //         tupleValues[i] = tupleElement != null ? tupleElement.ToJsonString() : "null";
    //     }
    //     return $"({string.Join(", ", tupleValues)})";
    // }
}
