using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using UEFA.ChampionsLeague.Contracts;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects;
using UEFA.ChampionsLeague.Contracts.Enums;

namespace UEFA.ChampionsLeague.Host.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _nextRequestDelegate;
        private readonly IHostingEnvironment _environment;

        public ExceptionHandlingMiddleware(RequestDelegate nextRequestDelegate, IHostingEnvironment environment)
        {
            _nextRequestDelegate = nextRequestDelegate ?? throw new ArgumentNullException(nameof(nextRequestDelegate));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.TraceIdentifier = Guid.NewGuid().ToString();
                await _nextRequestDelegate(context);
            }
            catch (UEFAException ex)
            {
                switch (ex.Type)
                {
                    case UEFAExceptionType.Business:
                        await HandleBusinessExceptionAsync(context, ex);
                        break;
                    case UEFAExceptionType.Data:
                        await HandleDataExceptionAsync(context, ex);
                        break;
                    default:
                        await HandleUnknownExceptionAsync(context, ex);
                        break;
                }
            }
            catch (Exception ex)
            {
                await HandleGlobalExceptionAsync(context, ex);
            }
        }

        private async Task HandleBusinessExceptionAsync(HttpContext context, UEFAException ex)
        {
            var response = new ResponseTemplateViewDto<bool> { ErrorMessage = ex.Message, IsSuccess = false };
            await WriteResponseAsync(context, HttpStatusCode.OK, JsonConvert.SerializeObject(response));
        }

        private async Task HandleDataExceptionAsync(HttpContext context, UEFAException ex)
        {
            var response = new ResponseTemplateViewDto<bool> { ErrorMessage = ex.Message, IsSuccess = false };
            await WriteResponseAsync(context, HttpStatusCode.OK, JsonConvert.SerializeObject(response));
        }

        private async Task HandleUnknownExceptionAsync(HttpContext context, UEFAException ex)
        {
            var response = new ResponseTemplateViewDto<bool> { ErrorMessage = ex.Message, IsSuccess = false };
            await WriteResponseAsync(context, HttpStatusCode.OK, JsonConvert.SerializeObject(response));
        }

        private async Task HandleGlobalExceptionAsync(HttpContext context, Exception ex)
        {
            if (!_environment.IsDevelopment())
            {
                try
                {
                    var response = new ResponseTemplateViewDto<bool> { IsSuccess = false, ErrorMessage = "Unhandled exception" };
                    await WriteResponseAsync(context, HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(response));
                }
                catch (Exception)
                {
                    var response = new ResponseTemplateViewDto<bool>
                    {
                        IsSuccess = false,
                        ErrorMessage = $"An unexpected error has occured. Please contact our customer support with the following code {context.TraceIdentifier}."
                    };
                    await WriteResponseAsync(context, HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(response));
                }
            }
            else
            {
                var response = new ResponseTemplateViewDto<bool> { IsSuccess = false, ErrorMessage = ex.Message };
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(response));
            }
        }

        private async Task WriteResponseAsync(HttpContext context, HttpStatusCode code, string jsonResponse)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
