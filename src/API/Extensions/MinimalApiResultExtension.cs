using Shared.Result;
using System.Net;
using System.Text.Json;

namespace API.Extensions;

/// <summary>
/// Extensão para converter ICommandResult em IResult do Minimal API.
/// Necessário porque Minimal APIs não executam IActionResult automaticamente —
/// serializam o objeto OkObjectResult inteiro em vez de executá-lo.
/// </summary>
public static class MinimalApiResultExtension
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static IResult ToMinimalResult(this ICommandResult result)
    {
        if (result.StatusCode == HttpStatusCode.NoContent)
            return Results.NoContent();

        return Results.Json(result, _jsonOptions, statusCode: (int)result.StatusCode);
    }
}
