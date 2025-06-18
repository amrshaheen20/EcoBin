using AutoMapper;
using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.BinContainer.Injector;
using EcoBin.API.Services.NotificationContainer;
using System.Net;

namespace EcoBin.API.Services.BinContainer
{
    public class BinService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        BinInjector binInjector,
        IHttpContextAccessor contextAccessor,
        NotificationService notificationService
        ) : IServiceInjector

    {
        private IGenericRepository<TranchBin> GetRepository()
        {
            return unitOfWork.GetRepository<TranchBin>().AddInjector(binInjector);
        }

        public async Task<IBaseResponse<BinResponseDto>> CreateBinAsync(BinRequestDto requestDto)
        {
            var Repository = GetRepository();

            var Entity = mapper.Map<TranchBin>(requestDto);
            Entity.CreatedById = contextAccessor.GetUserId();


            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return (await GetBinByIdAsync(Entity.Id))
                   .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<BinResponseDto>> GetBinByIdAsync(int binId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync<BinResponseDto>(binId);

            if (Entity == null)
            {
                return new BaseResponse<BinResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }

            return new BaseResponse<BinResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<BinResponseDto>> GetAllBins(PaginationFilter<BinResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<BinResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }


        public async Task<IBaseResponse<object>> UpdateBinAsync(int binId, BinRequestDto requestDto)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(binId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }

            mapper.Map(requestDto, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Bin updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteBinAsync(int binId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(binId);

            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }

            Repository.Delete(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Bin deleted successfully.");
        }


        public async Task<IBaseResponse<object>> EmptyBinAsync(int binId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(binId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }
            Entity.CurrentCapacity = 0;
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();
            await notificationService.SendBinStatusToManagerAsync(Entity);
            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Bin has been emptied successfully.");
        }

        public async Task<IBaseResponse<object>> ChangeLidStateAsync(int binId, LidStateRequestDto request)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(binId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }
            Entity.IsLidOpen = request.IsOpen;
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            await notificationService.SendBinStatusToManagerAsync(Entity);
            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Lid status has been changed successfully.");
        }


        public async Task<IBaseResponse<object>> ChangeCurrentCapacityAsync(int binId, CurrentCapacityRequestDto request)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(binId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }
            Entity.CurrentCapacity = request.CurrentCapacity;
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();
            await notificationService.SendBinStatusToManagerAsync(Entity);
            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Current capacity has been changed successfully.");
        }

        public async Task<IBaseResponse<BinTokenResponseDto>> CreateBinTokenAsync(int binId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(binId);
            if (Entity == null)
            {
                return new BaseResponse<BinTokenResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            Entity.Token = token;
            Repository.Update(Entity);


            return new BaseResponse<BinTokenResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(new BinTokenResponseDto
                {
                    Token = token,
                    BinId = Entity.Id
                });
        }


        public async Task<IBaseResponse<object>> SetBinStatusAsync(int binId, BinStatusRequestDto request)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(binId);
            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Bin not found");
            }

            if (request.Token != Entity.Token)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.Unauthorized)
                    .SetMessage("Invalid token.");
            }

            Entity.IsMaintenanceMode = request.IsMaintenanceMode;
            Entity.IsLidOpen = request.IsLidOpen;
            Entity.CurrentCapacity = request.CurrentCapacity;

            Repository.Update(Entity);
            await unitOfWork.SaveAsync();
            await notificationService.SendBinStatusToManagerAsync(Entity);
            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Bin status has been changed successfully.");
        }
    }
}
