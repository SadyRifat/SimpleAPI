using SimpleAPI.Dto;
using System.Net;
using System.Text.Json;

namespace SimpleAPI.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            ErrorResponse errorResponse = new ErrorResponse();
            BaseResponse<object> baseResponse = new BaseResponse<object>();

            switch (exception)
            {
                case ApplicationException ex:
                    errorResponse.Message = ex.Message;
                    errorResponse.Code = 500;
                    baseResponse.Error = errorResponse;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case FileNotFoundException ex:
                    errorResponse.Message = ex.Message;
                    errorResponse.Code = 500;
                    baseResponse.Error = errorResponse;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    errorResponse.Message = "Error Happened";
                    errorResponse.Code = 500;
                    baseResponse.Error = errorResponse;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

            }
            var exResult = JsonSerializer.Serialize(baseResponse);
            await context.Response.WriteAsync(exResult);
        }
    }
}
