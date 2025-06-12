using AutoMapper;
using EcoBin.API.Extensions;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;

namespace EcoBin.API.Services.BinContainer.Mapper
{
    public class BinProfile : Profile
    {
        public BinProfile()
        {
            CreateMap<BinRequestDto, TranchBin>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TranchBin, BinResponseDto>();
        }
    }
}
