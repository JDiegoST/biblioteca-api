namespace Biblioteca.Domain.Exceptions
{
    public class BadRequestException : ApiException
    {
        public override string Type => "http://api.biblioteca.com/errors/bad-request";

        public override int StatusCode => StatusCodes.Status400BadRequest;
        public override string Title => "Bad request";

        public BadRequestException(string message) : base(message) { }

        public BadRequestException(string message, object? extencions) : base(message, extencions) { }
    }
}
