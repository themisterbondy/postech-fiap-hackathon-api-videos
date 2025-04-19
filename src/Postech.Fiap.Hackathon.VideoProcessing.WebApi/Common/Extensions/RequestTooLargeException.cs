using Microsoft.AspNetCore.Mvc;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Extensions;

[ExcludeFromCodeCoverage]
public class RequestTooLargeException(RequestDelegate next, IConfiguration config)
{
    public async Task InvokeAsync(HttpContext context)
    {
        long.TryParse(config["MultipartBodyLengthLimit"], out var MultipartBodyLengthLimit);

        if (context.Request.ContentLength.HasValue && context.Request.ContentLength > MultipartBodyLengthLimit)
        {
            var error = Error.Validation("Invalid.Input", "Request is too large");
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#status.400",
                Title = "Bad Request",
                Detail = "One or more validation errors occurred.",
                Extensions =
                {
                    {
                        "errors", new[] { error }
                    }
                }
            };
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails);
            return;
        }


        await next(context);
    }
}