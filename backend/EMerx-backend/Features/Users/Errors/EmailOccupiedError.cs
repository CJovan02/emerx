using FluentResults;

namespace EMerx_backend.Features.Users.Errors;

public class EmailOccupiedError : Error
{
    public EmailOccupiedError(string email) : base($"Email {email} already exists.")
    {
        Metadata.Add("StatusCode", 409);
    }
}