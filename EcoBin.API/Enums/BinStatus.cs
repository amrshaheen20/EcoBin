using System.Text.Json.Serialization;

namespace EcoBin.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eBinStatus
    {
        Unknown = 0,
        Empty = 1,
        Partial = 2,
        Full = 3,
        Overfilled = 4,
    }
}
