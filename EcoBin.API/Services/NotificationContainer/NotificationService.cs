using EcoBin.API.Common;
using EcoBin.API.Enums;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.NotificationContainer.Injector;
using System.Net;

namespace EcoBin.API.Services.NotificationContainer
{
    public class NotificationService(
        IUnitOfWork unitOfWork,
        NotificationInjector notificationInjector
        ) : IServiceInjector

    {

        private IGenericRepository<Notification> GetRepository()
        {
            return unitOfWork.GetRepository<Notification>().AddInjector(notificationInjector);
        }

        public async Task<IBaseResponse<NotificationResponseDto>> GetNotificationByIdAsync(int notificationId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync<NotificationResponseDto>(notificationId);

            if (Entity == null)
            {
                return new BaseResponse<NotificationResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Notification not found");
            }

            return new BaseResponse<NotificationResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<NotificationResponseDto>> GetAllNotifications(PaginationFilter<NotificationResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<NotificationResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }




        public async Task SendBinStatusToManagerAsync(TranchBin bin)
        {
            string message = $"Bin {bin.Name} at {bin.Location} has a status update. " +
                             $"Current capacity: {bin.CurrentCapacity}/{bin.MaxCapacity}. " +
                             $"Status of bin is: {bin.Status.ToString()}.";
            var notification = new Notification
            {
                Title = "Bin Status Update",
                Message = message,
                RecipientUserId = bin.CreatedById,
                Type = eNotificationType.BinStatusUpdate
            };

            var NotificationRepository = GetRepository();
            await NotificationRepository.AddAsync(notification);
            await unitOfWork.SaveAsync();

        }



        public async Task SendReportToManger(Report report)
        {
            string message = $"A new report has been created with the following details: " +
                             $"Title: {report.Title} " +
                             $"Description: {report.Message}";
            var notification = new Notification
            {
                Title = "New Report Created",
                Message = message,
                RecipientUserId = report.MangerId,
                Type = eNotificationType.ReportIssue

            };
            var NotificationRepository = GetRepository();
            await NotificationRepository.AddAsync(notification);
            await unitOfWork.SaveAsync();
        }

        public async Task SendTaskStatusToManger(WorkerTask task)
        {
            string message = $"Task '{task.Title}' " +
                             $"has been updated. " +
                             $"Current status: {(task.IsCompleted ? "Completed" : "Pending")}.";

            var notification = new Notification
            {
                Title = "Task Status Update",
                Message = message,
                RecipientUserId = task.CreatedById,
                Type = eNotificationType.TaskCompletion
            };

            var NotificationRepository = GetRepository();
            await NotificationRepository.AddAsync(notification);
            await unitOfWork.SaveAsync();

        }
    }
}
