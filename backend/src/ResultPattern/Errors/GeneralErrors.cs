namespace EMerx.ResultPattern.Errors;

public static class GeneralErrors
{
    public static Error DatabaseError() => new(StatusCodes.InternalServerError, "Database error");
}