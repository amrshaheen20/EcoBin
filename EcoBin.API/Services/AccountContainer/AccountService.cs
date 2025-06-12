using AutoMapper;
using EcoBin.API.Common;
using EcoBin.API.Enums;
using EcoBin.API.Extensions;
using EcoBin.API.Interfaces;
using EcoBin.API.Models.DbSet;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.AccountContainer.Injector;
using EcoBin.API.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace EcoBin.API.Services.AccountContainer
{
    public class AccountService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        JwtService jwtService,
        IHttpContextAccessor contextAccessor,
        AccountInjector accountInjector,
        IMemoryCache cache,
        EmailSenderService emailSenderService
        ) : IServiceInjector

    {
        private IGenericRepository<User> GetRepository()
        {
            accountInjector.InjectMangerCommands();
            return unitOfWork.GetRepository<User>().AddInjector(accountInjector);
        }

        public async Task<IBaseResponse<AccountResponseDto>> CreateAccountAsync(AccountRequestDto account)
        {
            var userRepository = GetRepository();

            if (await userRepository.AnyAsync(x => x.Email == account.Email))
            {
                return new BaseResponse<AccountResponseDto>()
                    .SetStatus(HttpStatusCode.Conflict)
                    .SetMessage("Email already exists.");
            }

            var userEntity = mapper.Map<User>(account);

            await userRepository.AddAsync(userEntity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<AccountResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<AccountResponseDto>(userEntity));
        }



        public async Task<IBaseResponse<AccountResponseDto>> GetAccountByIdAsync(int accountId)
        {
            var userEntity = await GetRepository().GetByIdAsync<AccountResponseDto>(accountId);

            if (userEntity == null)
            {
                return new BaseResponse<AccountResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Account not found.");
            }

            return new BaseResponse<AccountResponseDto>()
                .SetData(userEntity);
        }


        public IBaseResponse<PaginateBlock<AccountResponseDto>> GetAllAccounts(PaginationFilter<AccountResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<AccountResponseDto>>()
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<object>> UpdateAccountAsync(int accountId, AccountRequestDto updatedAccount)
        {
            var userRepository = GetRepository();
            var userEntity = await userRepository.GetByIdAsync(accountId);

            if (userEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Account not found.");
            }

            mapper.Map(updatedAccount, userEntity);

            userRepository.Update(userEntity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Account updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteAccountByIdAsync(int accountId)
        {
            var userRepository = GetRepository();
            var userEntity = await userRepository.GetByIdAsync(accountId);

            if (userEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Account not found.");
            }

            userRepository.Delete(userEntity);

            await unitOfWork.SaveAsync();
            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Account deleted successfully.");
        }

        #region Auth

        public async Task<IBaseResponse<bool>> IsUserOnline(int userId)
        {
            var user = await GetRepository().GetByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponse<bool>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("User not found.");
            }

            var IsOnline = user.LastActiveTime <= DateTime.UtcNow.AddMinutes(10);

            return new BaseResponse<bool>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(IsOnline);
        }

        public IBaseResponse<AccountResponseDto> GetAccountByAuthAsync()
        {
            return new BaseResponse<AccountResponseDto>()
                .SetData(mapper.Map<AccountResponseDto>(contextAccessor.GetCurrentUser()));
        }


        public async Task<IBaseResponse<AccountResponseDto>> RegisterAsync(RegisterRequestDto account)
        {
            var userRepository = GetRepository();
            if (await userRepository.AnyAsync(x => x.Email == account.Email))
            {
                return new BaseResponse<AccountResponseDto>()
                    .SetStatus(HttpStatusCode.Conflict)
                    .SetMessage("Email already exists.");
            }
            var userEntity = mapper.Map<User>(account);
            userEntity.Role = eRole.Manger;

            userEntity.PasswordHash = account.Password!.HashPassword();
            await userRepository.AddAsync(userEntity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<AccountResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("User registered successfully.");
        }


        public async Task<IBaseResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginInfo)
        {
            var injector = new CommandsInjector<User>();
            injector.Where(x => x.Email == loginInfo.Email);

            var user = await GetRepository().GetByAsync(injector);

            if (user == null || !user.PasswordHash.VerifyPassword(loginInfo.Password))
            {
                return new BaseResponse<LoginResponseDto>()
                    .SetStatus(HttpStatusCode.Unauthorized)
                    .SetMessage("Invalid credentials");
            }


            return await GenerateTokenAsync(user);
        }

        public async Task<IBaseResponse<object>> Logout()
        {
            var tokenJti = contextAccessor.GetTokenJti();

            await unitOfWork.GetRepository<UserSession>()
                .GetAll()
                .Where(x => x.TokenId == tokenJti)
                .ExecuteDeleteAsync();

            return new BaseResponse<object>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("User is logged out.");
        }


        private bool VerifyToken(string Email, string token)
        {
            var cachedToken = cache.Get<string>(reset_password_id(Email));
            return cachedToken != null && cachedToken == token;
        }


        private readonly Func<string, string> reset_password_id = email => $"{email}_reset_password";

        public async Task<IBaseResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var repository = GetRepository();
            var user = await repository.GetByAsync(new CommandsInjector<User>().Where(x => x.Email == request.Email));
            if (user == null)
            {
                return new BaseResponse<object>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("User not found.");
            }

            var resetToken = Random.Shared.Next(10000, 99999).ToString();

            cache.Set(reset_password_id(user.Email), resetToken, TimeSpan.FromMinutes(15));


            await emailSenderService.SendPasswordResetAsync(user.Email, resetToken);

            return new BaseResponse<object>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Password reset instructions have been sent to your email.");
        }

        public async Task<IBaseResponse<object>> VerifyResetTokenAsync(VerifyResetTokenRequestDto request)
        {
            var repository = GetRepository();
            var user = await repository.GetByAsync(new CommandsInjector<User>().Where(x => x.Email == request.Email));
            if (user == null)
            {
                return new BaseResponse<object>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("User not found.");
            }
            if (!VerifyToken(request.Email, request.Token))
            {
                return new BaseResponse<object>()
                    .SetStatus(HttpStatusCode.BadRequest)
                    .SetMessage("Invalid or expired reset token.");
            }
            return new BaseResponse<object>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Reset token is valid.");
        }

        public async Task<IBaseResponse<LoginResponseDto>> ChangePasswordAsync(ResetPasswordRequestDto request)
        {
            var repository = GetRepository();
            var user = await repository.GetByAsync(new CommandsInjector<User>().Where(x => x.Email == request.Email));
            if (user == null)
            {
                return new BaseResponse<LoginResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("User not found.");
            }

            if (!VerifyToken(request.Email, request.Token))
            {
                return new BaseResponse<LoginResponseDto>()
                    .SetStatus(HttpStatusCode.BadRequest)
                    .SetMessage("Invalid or expired reset token.");
            }

            cache.Remove(reset_password_id(user.Email));

            user.PasswordHash = request.NewPassword.HashPassword();
            repository.Update(user);
            await LogOutUserFromAllSessionsAsync(user.Id);
            await unitOfWork.SaveAsync();


            return await GenerateTokenAsync(user);
        }

        private async Task LogOutUserFromAllSessionsAsync(int userId)
        {
            await unitOfWork.GetRepository<UserSession>()
                .GetAll()
                .Where(x => x.UserId == userId)
                .ExecuteDeleteAsync();
        }

        private async Task<IBaseResponse<LoginResponseDto>> GenerateTokenAsync(User user)
        {
            var token = jwtService.GenerateJwtToken(user);

            StringValues userAgentValues;
            contextAccessor.HttpContext!.Request.Headers.TryGetValue(HeaderNames.UserAgent, out userAgentValues);
            var userSession = new UserSession
            {
                UserId = user.Id,
                TokenId = token.Token.Id,
                ExpirationTime = token.Token.ValidTo,
                UserAgent = userAgentValues.ToString()
            };

            await unitOfWork.GetRepository<UserSession>().AddAsync(userSession);
            await unitOfWork.SaveAsync();

            return new BaseResponse<LoginResponseDto>()
                .SetData(new LoginResponseDto
                {
                    Token = token.GenerateToken(),
                    Expiration = token.Token.ValidTo,
                    User = mapper.Map<AccountResponseDto>(user)
                });
        }
        #endregion
    }
}
