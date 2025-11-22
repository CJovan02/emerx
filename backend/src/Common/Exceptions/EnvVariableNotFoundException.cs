namespace EMerx.Common.Exceptions;

public class EnvVariableNotFoundException : Exception
{
    public EnvVariableNotFoundException(string envVariable) : base($"Environmnt variable {envVariable} not found!") { }
}