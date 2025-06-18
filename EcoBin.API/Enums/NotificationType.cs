using System.Text.Json.Serialization;

namespace EcoBin.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eNotificationType
    {
        Info = 0,
        BinStatusUpdate = 1,
        TaskCompletion = 2,
        ReportIssue = 3,
    }
}
