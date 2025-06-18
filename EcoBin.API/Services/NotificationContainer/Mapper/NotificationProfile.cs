using AutoMapper;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;

namespace EcoBin.API.Services.NotificationContainer.Mapper
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationResponseDto>();
        }
    }
}
