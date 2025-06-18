using EcoBin.API.Common;
using EcoBin.API.Enums;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;

namespace EcoBin.API.Services.NotificationContainer.Injector
{
    public class NotificationInjector : CommandsInjector<Notification>, IServiceInjector
    {
        public NotificationInjector(IHttpContextAccessor contextAccessor)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();


            switch (UserRole)
            {
                case eRole.Worker:
                case eRole.Manger:
                    Where(x => x.RecipientUserId == UserId);
                    break;

                default:
                    Where(x => x.Id == 0); //return empty list
                    break;
            }

            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}
