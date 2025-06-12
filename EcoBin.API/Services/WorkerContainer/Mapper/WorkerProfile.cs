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
                    var user = context.Mapper.Map<User>(src);
                    user.Role = eRole.Worker;
                    return user;
                }))

                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Worker, WorkerResponseDto>()
                .ForMember(dest => dest.jobType, opts => opts.MapFrom(src => src.jobType))
                .ForMember(dest => dest.Account, opts => opts.MapFrom(src => src.User));
        }
    }
}
