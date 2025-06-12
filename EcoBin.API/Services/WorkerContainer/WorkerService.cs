using AutoMapper;
using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.WorkerContainer.Injector;
using System.Net;

namespace EcoBin.API.Services.WorkerContainer
{
    public class WorkerService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        WorkerInjector workerInjector,
        IHttpContextAccessor contextAccessor
        ) : IServiceInjector

    {
        private IGenericRepository<Worker> GetRepository()
        {
            return unitOfWork.GetRepository<Worker>().AddInjector(workerInjector);
        }

        public async Task<IBaseResponse<WorkerResponseDto>> CreateWorkerAsync(WorkerRequestDto requestDto)
        {
            var Repository = GetRepository();

            var Entity = mapper.Map<Worker>(requestDto);

            Entity.CreatedById = contextAccessor.GetUserId();

            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return (await GetWorkerByIdAsync(Entity.Id))
                   .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<WorkerResponseDto>> GetWorkerByIdAsync(int WorkerId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync<WorkerResponseDto>(WorkerId);

            if (Entity == null)
            {
                return new BaseResponse<WorkerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Worker not found");
            }

            return new BaseResponse<WorkerResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<WorkerResponseDto>> GetAllWorkers(PaginationFilter<WorkerResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<WorkerResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }


        public async Task<IBaseResponse<object>> UpdateWorkerAsync(int WorkerId, WorkerRequestDto requestDto)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(WorkerId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Worker not found");
            }

            mapper.Map(requestDto, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Worker updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteWorkerAsync(int WorkerId)
        {
            var Repository = GetRepository();
            var accountRepo = unitOfWork.GetRepository<User>();
            var entity = await Repository.GetByIdAsync(WorkerId);

            if (entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Worker not found");
            }

            Repository.Delete(entity);
            var user = await accountRepo.GetByIdAsync(entity.UserId);
            if (user != null)
            {
                accountRepo.Delete(user);
            }
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Worker deleted successfully.");
        }

    }
}
