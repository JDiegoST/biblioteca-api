namespace Biblioteca.Domain.Exceptions;

public class BusinesException : ApiException
{
    public override string Type => "http://api.biblioteca.com/errors/busines-error";
    public override int StatusCode => StatusCodes.Status422UnprocessableEntity;
    public override string Title => "BUSINES_ERROR";

    public BusinesException(string message) : base(message) { }

    public BusinesException(string message, object? extencions) : base(message, extencions) {}
}