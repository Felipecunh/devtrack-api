using DevTrack.API.Helpers;
using System.Net;
using System.Text.Json;

namespace DevTrack.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Loga o erro
            _logger.LogError(ex, "Unhandled exception occurred");

            // Chama o método para tratar a exceção
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Resposta padrão para erros inesperados
        var response = ApiResponse<string>.Fail(
            "An unexpected error occurred. Please try again later."
        );

        // Cria um objeto com detalhes sobre o erro para fins de log
        var errorDetails = new
        {
            message = ex.Message,
            stackTrace = ex.StackTrace
        };

        // Log opcional: em um ambiente de produção, você pode querer gravar em sistemas centralizados de monitoramento como Sentry ou Azure Application Insights
        // _logger.LogError("Exception Details: " + JsonSerializer.Serialize(errorDetails));

        // Serializa a resposta e escreve na resposta HTTP
        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}
