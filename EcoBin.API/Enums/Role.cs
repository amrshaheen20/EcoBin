using System.Text.Json.Serialization;

namespace EcoBin.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eRole
    {
        None = 0,
        Manger,
        Worker
    }

}
