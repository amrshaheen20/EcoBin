using EcoBin.API.Common;
using EcoBin.API.Enums;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;

namespace EcoBin.API.Services.TaskContainer.Injector
{
    public class TaskInjector : CommandsInjector<WorkerTask>, IServiceInjector
    {
        public TaskInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {

            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();


            switch (UserRole)
            {
                case eRole.Worker:
                    Where(x => x.Worker.UserId == UserId);
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