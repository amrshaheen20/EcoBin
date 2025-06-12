using EcoBin.API.Common;
using EcoBin.API.Enums;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using Microsoft.AspNetCore.Authorization;

namespace EcoBin.API.Services.AccountContainer.Injector
{
    public class AccountInjector : CommandsInjector<User>, IServiceInjector
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUnitOfWork unitOfWork;

        public AccountInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            AddCommand(q => q.OrderByDescending(x => x.Id));
            this.contextAccessor = contextAccessor;
            this.unitOfWork = unitOfWork;
        }


        public void InjectMangerCommands()
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
                case eRole.Manger:
                    var workers = unitOfWork.GetRepository<Worker>().GetAll()
                        .Where(x => x.CreatedById == UserId)
                        .Select(x => x.UserId)
                        .ToList();

                    Where(x => workers.Contains(x.Id) || x.Id == UserId); //include all workers and the manager itself
                    break;

                default:
                    Where(x => x.Id == 0); //return empty list
                    break;
            }

            AddCommand(q => q.OrderByDescending(x => x.Id));

        }
    }
}
