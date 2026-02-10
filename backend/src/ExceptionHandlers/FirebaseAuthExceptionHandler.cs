using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.ExceptionHandlers;

public class FirebaseAuthExceptionHandler(ILogger<FirebaseAuthExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<FirebaseAuthExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not FirebaseAuthException firebaseAuthException)
            return false;

        _logger.LogError(exception, "Firebase Auth Exception Occurred {Message}", firebaseAuthException.Message);
        _logger.LogInformation(firebaseAuthException.AuthErrorCode.ToString());

        var problemDetails = new ProblemDetails
        {
            Detail = firebaseAuthException.Message,
            Title = "Firebase Auth Error",
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}