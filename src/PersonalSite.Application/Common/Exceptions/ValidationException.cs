namespace PersonalSite.Application.Common.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(IReadOnlyList<ValidationFailure> failures)
        : base("One or more validation failures occurred.")
    {
        Failures = failures;
    }

    public IReadOnlyList<ValidationFailure> Failures { get; }
}

public sealed record ValidationFailure(string PropertyName, string Message);
