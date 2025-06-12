using System.Text.Json.Serialization;

namespace EcoBin.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eWorkerJobType
    {
        GarbageCollector = 0,
        GarbageBinMaintenance,
        GarbageTruckDriver,
    }
}
