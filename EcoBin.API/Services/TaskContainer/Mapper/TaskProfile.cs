using AutoMapper;
using EcoBin.API.Enums;
using EcoBin.API.Extensions;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;

namespace EcoBin.API.Services.TaskContainer.Mapper
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<WorkerTaskRequestDto, WorkerTask>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<WorkerTask, WorkerTaskResponseDto>();
        }
    }
}
