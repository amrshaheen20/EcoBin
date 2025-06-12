using AutoMapper;
using EcoBin.API.Extensions;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;

namespace EcoBin.API.Services.AccountContainer.Mapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountRequestDto, User>()

                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom((src, dest) => src.Role ?? dest.Role))
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom((src, dest) => src.Password != null ? src.Password.HashPassword() : dest.PasswordHash))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom((src, dest) => dest.Role))
               .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom((src, dest) => src.Password != null ? src.Password.HashPassword() : dest.PasswordHash))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<User, AccountResponseDto>();
        }
    }
}
