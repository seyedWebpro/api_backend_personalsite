namespace PersonalSite.Application.Common.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string name, object key)
        : base($"{name} '{key}' was not found.")
    {
    }
}
