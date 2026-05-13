namespace EMerx.Common.Exceptions;

public class UserNotFoundById(string id) : Exception($"User with id: {id} not found");