using EcoBin.API;
using EcoBin.API.Extensions;
using EcoBin.API.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSwaggerSupport(builder.Configuration);
        builder.Services.AddApiConfigurationExtensions();
        builder.Services.AddJwtConfiguration(builder);
        builder.Services.AddAutoServices();
        builder.Services.AddMemoryCache();



        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = CustomInvalidModelResponse.Generate;
        });

        var authObj = builder.Services.AddAuthorizationBuilder();
        foreach (var policy in Policies.PolicyRolesMap)
        {
            authObj.AddPolicy(policy.Key, pol => pol.RequireRole(policy.Value));
        }

        var app = builder.Build();



        app.UseMiddleware<ResponseTimeMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();

        if (builder.Configuration.GetValue<bool>("FeatureFlags:MigrateAtStartup"))
        {
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        dbContext.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while migrating or seeding the database.");
                }
            }
        }


        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            c.ConfigObject.AdditionalItems.Add("deepLinking", "true");
        });

        app.UseRouting();
        app.UseErrorHandler();

        app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}