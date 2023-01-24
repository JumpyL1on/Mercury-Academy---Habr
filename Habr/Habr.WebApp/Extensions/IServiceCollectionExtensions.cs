using FluentValidation;
using FluentValidation.AspNetCore;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;
using Habr.BusinessLogic.Validators;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Habr.WebApp.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;

namespace Habr.WebApp.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddScoped<ICommentService, CommentService>()
                .AddScoped<IDraftPostService, DraftPostService>()
                .AddTransient<IJwtService, JwtService>()
                .AddScoped<IPostService, PostService>()
                .AddScoped<IPublishedPostService, PublishedPostService>()
                .AddScoped<IRatingService, RatingService>()
                .AddScoped<IUserService, UserService>();
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

            return services.AddFluentValidation(config =>
            {
                var assembly = typeof(CreatePostRequestValidator).Assembly;

                config.RegisterValidatorsFromAssembly(assembly);
            });
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };

                    options.MapInboundClaims = false;
                });

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, Role>(options =>
                {
                    options.User.AllowedUserNameCharacters = "";
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                })
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddAPIVersioning(this IServiceCollection services)
        {
            return services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen();

            return services.ConfigureOptions<ConfigureSwaggerOptions>();
        }

        public static IServiceCollection AddAPIControllers(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(typeof(ExceptionFilter));
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var entry = context.ModelState.Values.First(value => value.Errors.Count != 0);

                        return new BadRequestObjectResult(entry.Errors[0].ErrorMessage);
                    };
                });

            return services;
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(cfg =>
            {
                var connectionString = configuration.GetConnectionString("HangfireConnection");

                cfg.UseSqlServerStorage(connectionString);

                cfg.UseColouredConsoleLogProvider();
            });

            return services.AddHangfireServer();
        }
    }
}