using Habr.BusinessLogic.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Habr.WebApp.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
        {
            return app.UseSwaggerUI(options =>
            {
                using var scope = app.ApplicationServices.CreateScope();

                var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName);
                }
            });
        }

        public static IApplicationBuilder AddRecurringJob(this IApplicationBuilder app, IConfiguration configuration)
        {
            using var scope = app.ApplicationServices.CreateScope();
            {
                var manager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                manager.AddOrUpdate<IRatingService>(
                    "jobId",
                    ratingService => ratingService.UpdateAverageRatingForAllPublishedPostsAsync(),
                    configuration["JobScheduleInCronFormat:UpdateAverageRatingForAllPublishedPosts"],
                    TimeZoneInfo.Utc);
            }

            return app;
        }
    }
}