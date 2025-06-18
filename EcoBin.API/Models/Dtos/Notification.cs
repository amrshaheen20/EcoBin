using EcoBin.API.Enums;

namespace EcoBin.API.Models.Dtos
{
    public class NotificationResponseDto:BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public eNotificationType Type { get; set; } = eNotificationType.Info;
    }
}
