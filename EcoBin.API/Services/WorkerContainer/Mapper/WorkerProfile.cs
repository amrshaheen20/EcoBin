using AutoMapper;
using EcoBin.API.Enums;
using EcoBin.API.Extensions;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;

namespace EcoBin.API.Services.WorkerContainer.Mapper
{
    public class WorkerProfile : Profile
    {
        public WorkerProfile()
        {
            CreateMap<WorkerRequestDto, Worker>()
                .ForMember(dest => dest.jobType, opts => opts.MapFrom(src => src.jobType))
                .ForMember(dest => dest.User, opts => opts.MapFrom((src, dest, destMember, context) =>
                {
                    var user = context.Mapper.Map(src, dest.User);
                    user.Role = eRole.Worker;
                    return user;
                }))

                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Worker, WorkerResponseDto>()
                .ForMember(dest => dest.jobType, opts => opts.MapFrom(src => src.jobType))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedAt, opts => opts.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.AccountId, opts => opts.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Role, opts => opts.MapFrom(src => src.User.Role))
                .ForMember(dest => dest.PhoneNumber, opts => opts.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.CompanyName, opts => opts.MapFrom(src => src.User.CompanyName))
                .ForMember(dest => dest.LastActiveTime, opts => opts.MapFrom(src => src.User.LastActiveTime));
        }
    }
}
