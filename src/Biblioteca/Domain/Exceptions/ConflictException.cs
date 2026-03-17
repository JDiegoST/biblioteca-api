namespace Biblioteca.Domain.Exceptions;

public class ConflictException : ApiException
{
    public override string Type => "http://api.biblioteca.com/errors/conflict";
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "CONFLICT";

    public ConflictException(string message) : base(message) { }
    public ConflictException(string message, object? extencions ) : base(message, extencions) { }
}