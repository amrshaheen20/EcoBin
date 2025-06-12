using System.Text.Json.Serialization;

namespace EcoBin.API.Models.Dtos
{
    public class BaseResponseDto
    {
        [JsonPropertyOrder(-1)]
        public int Id { get; set; }

        [JsonPropertyOrder(1000)]
        public DateTime CreatedAt { get; set; }
    }
}
