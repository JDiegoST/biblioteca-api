namespace Biblioteca.Domain.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public override string Type => "http://api.biblioteca.com/errors/unauthorized-access";
        public override int StatusCode => StatusCodes.Status403Forbidden;
        public override string Title => "Acceso no autorizado";

        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, object? extentions) : base(message, extentions) { }

    }
}
