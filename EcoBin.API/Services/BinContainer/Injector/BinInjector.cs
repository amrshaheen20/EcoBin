using EcoBin.API.Common;
using EcoBin.API.Enums;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using Microsoft.AspNetCore.Authorization;

namespace EcoBin.API.Services.BinContainer.Injector
{
    public class BinInjector : CommandsInjector<TranchBin>, IServiceInjector
    {
        public BinInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            var endpoint = contextAccessor.HttpContext?.GetEndpoint();
            var hasIgnoreAuth = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;
            if (hasIgnoreAuth)
            {
                return;
            }

            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();


            switch (UserRole)
            {
                case eRole.Worker:
                    var worker = unitOfWork.GetRepository<Worker>().GetAll()
                        .FirstOrDefault(x => x.UserId == UserId);

                    Where(x => x.CreatedById == worker!.CreatedById);
                    break;

                case eRole.Manger:
                    Where(x => x.CreatedById == UserId);
                    break;

                default:
                    Where(x => x.Id == 0); //return empty list
                    break;
            }

            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}