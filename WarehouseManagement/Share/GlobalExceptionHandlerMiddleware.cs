namespace WarehouseManagement.Share
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = new { error = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
