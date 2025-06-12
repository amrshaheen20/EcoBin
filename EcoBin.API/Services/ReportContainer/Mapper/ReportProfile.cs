using AutoMapper;
using EcoBin.API.Extensions;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;

namespace EcoBin.API.Services.ReportContainer.Mapper
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<ReportRequestDto, Report>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Report, ReportResponseDto>();
        }
    }
}
