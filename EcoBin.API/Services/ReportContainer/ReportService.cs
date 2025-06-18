using AutoMapper;
using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.NotificationContainer;
using EcoBin.API.Services.ReportContainer.Injector;
using System.Net;

namespace EcoBin.API.Services.ReportContainer
{
    public class ReportService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ReportInjector reportInjector,
        IHttpContextAccessor contextAccessor,
        NotificationService notificationService
        ) : IServiceInjector

    {
        private IGenericRepository<Report> GetRepository()
        {
            return unitOfWork.GetRepository<Report>().AddInjector(reportInjector);
        }

        public async Task<IBaseResponse<ReportResponseDto>> CreateReportAsync(ReportRequestDto requestDto)
        {
            var Repository = GetRepository();

            var worker = unitOfWork.GetRepository<Worker>().GetAll().First(x => x.UserId == contextAccessor.GetUserId());

            var Entity = mapper.Map<Report>(requestDto);
            Entity.WorkerId = worker.Id;
            Entity.MangerId = worker.CreatedById;

            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            await notificationService.SendReportToManger(Entity);

            return (await GetReportByIdAsync(Entity.Id))
                   .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<ReportResponseDto>> GetReportByIdAsync(int reportId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync<ReportResponseDto>(reportId);

            if (Entity == null)
            {
                return new BaseResponse<ReportResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Report not found");
            }

            return new BaseResponse<ReportResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<ReportResponseDto>> GetAllReports(PaginationFilter<ReportResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<ReportResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }


        public async Task<IBaseResponse<object>> UpdateReportAsync(int reportId, ReportRequestDto requestDto)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(reportId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Report not found");
            }

            mapper.Map(requestDto, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Report updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteReportAsync(int reportId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(reportId);

            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Report not found");
            }

            Repository.Delete(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Report deleted successfully.");
        }

    }
}
