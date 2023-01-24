using FluentValidation.AspNetCore;
using Habr.BusinessLogic.Profiles;
using Habr.DataAccess;
using Habr.WebApp.Extensions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

LogManager
    .Setup()
    .LoadConfigurationFromAppSettings();

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Logging
    .ClearProviders()
    .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

webApplicationBuilder.Host.UseNLog();

webApplicationBuilder.Services.AddDbContext<DbContext, DataContext>();

webApplicationBuilder.Services.AddIdentity();

webApplicationBuilder.Services.AddServices();

webApplicationBuilder.Services.AddAutoMapper(typeof(PostProfile).Assembly);

webApplicationBuilder.Services.AddJwtAuthentication(webApplicationBuilder.Configuration);

webApplicationBuilder.Services.AddAPIControllers();

webApplicationBuilder.Services.AddFluentValidation();

webApplicationBuilder.Services.AddAPIVersioning();

webApplicationBuilder.Services.AddSwagger();

//webApplicationBuilder.Services.AddHangfire(webApplicationBuilder.Configuration);

var webApplication = webApplicationBuilder.Build();

//webApplication.AddRecurringJob(webApplicationBuilder.Configuration);

if (webApplication.Environment.IsDevelopment())
{
    webApplication.UseSwagger();

    webApplication.UseSwaggerUI();
}

webApplication.UseHttpsRedirection();

webApplication.UseAuthentication();

webApplication.UseAuthorization();

webApplication.MapControllers();

//webApplication.UseHangfireDashboard();

webApplication.Run();

LogManager.Shutdown();