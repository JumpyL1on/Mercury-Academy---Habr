using Habr.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace Habr.WebApp
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case BusinessException:
                case SecurityTokenException:
                    HandleException(context, StatusCodes.Status400BadRequest);

                    break;
                case ForbiddenException:
                    HandleException(context, StatusCodes.Status403Forbidden);

                    break;
                case NotFoundException:
                    HandleException(context, StatusCodes.Status404NotFound);

                    break;
                default:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    context.Result = new ContentResult()
                    {
                        Content = context.Exception.Message
                    };

                    var ex = context.Exception;

                    _logger.LogError($"{StatusCodes.Status500InternalServerError} {ex.Message} {ex.StackTrace}");

                    break;
            }
        }

        private void HandleException(ExceptionContext context, int status)
        {
            context.HttpContext.Response.StatusCode = status;

            context.Result = new ContentResult()
            {
                Content = context.Exception.Message
            };

            _logger.LogWarning($"{status} {context.Exception.Message}");
        }
    }
}