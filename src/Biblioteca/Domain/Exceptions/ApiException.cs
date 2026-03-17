
namespace Biblioteca.Domain.Exceptions;

public abstract class ApiException : Exception
{
    public abstract string Type { get; }
    public abstract int StatusCode { get; }
    public abstract string Title { get; }
    public object? Extencions { get; }

    protected ApiException(string message) : base(message) { }
    protected ApiException(string message, object? extencions)
        : base(message)
    {
        Extencions = extencions;
    }

}
