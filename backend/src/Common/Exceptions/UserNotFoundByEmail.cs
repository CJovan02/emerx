namespace EMerx.Common.Exceptions;

public class UserNotFoundByEmail(string email) : Exception($"User with email: {email} not found");