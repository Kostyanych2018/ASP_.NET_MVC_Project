using FluentValidation.Results;

namespace GameStore.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(string propertyName, string errorMessage)
        : base(errorMessage)
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }

    public ValidationException(IEnumerable<ValidationFailure> errors) : this()
    {
        Errors = errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}