using DevTrack.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevTrack.API.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var error = context.ModelState
                .SelectMany(x => x.Value!.Errors)
                .FirstOrDefault()?.ErrorMessage
                ?? "Invalid request data";

            context.Result = new BadRequestObjectResult(
                ApiResponse<string>.Fail(error)
            );
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
