using Microsoft.AspNetCore.Mvc;

namespace DevTrack.API.Helpers;

public static class ValidationResponse
{
    public static IActionResult HandleInvalidModelState(ActionContext context)
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(
            ApiResponse<List<string>>.Fail(string.Join(" | ", errors))
        );
    }
}
