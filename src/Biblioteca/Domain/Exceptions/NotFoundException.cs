namespace Biblioteca.Domain.Exceptions;

public class NotFoundException : ApiException
{

    public override string Type => "http://api.biblioteca.com/errors/not-found";
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "NOT_FOUND";

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, object exceptions ) : base(message, exceptions) { }
}