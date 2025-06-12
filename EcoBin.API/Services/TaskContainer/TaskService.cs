using AutoMapper;
using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.TaskContainer.Injector;
using System.Net;

namespace EcoBin.API.Services.TaskContainer
{
    public class TaskService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TaskInjector taskInjector,
        IHttpContextAccessor contextAccessor
        ) : IServiceInjector

    {
        private IGenericRepository<WorkerTask> GetRepository()
        {
            return unitOfWork.GetRepository<WorkerTask>().AddInjector(taskInjector);
        }

        public async Task<IBaseResponse<WorkerTaskResponseDto>> CreateTaskAsync(WorkerTaskRequestDto requestDto)
        {
            var Repository = GetRepository();

            var Entity = mapper.Map<WorkerTask>(requestDto);

            Entity.CreatedById = contextAccessor.GetUserId();

            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return (await GetTaskByIdAsync(Entity.Id))
                   .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<WorkerTaskResponseDto>> GetTaskByIdAsync(int TaskId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync<WorkerTaskResponseDto>(TaskId);

            if (Entity == null)
            {
                return new BaseResponse<WorkerTaskResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Task not found");
            }

            return new BaseResponse<WorkerTaskResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<WorkerTaskResponseDto>> GetAllTasks(PaginationFilter<WorkerTaskResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<WorkerTaskResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }


        public async Task<IBaseResponse<object>> UpdateTaskAsync(int TaskId, WorkerTaskRequestDto requestDto)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(TaskId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Task not found");
            }

            mapper.Map(requestDto, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Task updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteTaskAsync(int TaskId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(TaskId);

            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Task not found");
            }

            Repository.Delete(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Task deleted successfully.");
        }

    }
}
