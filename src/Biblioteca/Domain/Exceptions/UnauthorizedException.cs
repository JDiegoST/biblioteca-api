namespace Biblioteca.Domain.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public override string Type => "http://api.biblioteca.com/errors/unauthorized-access";
        public override int StatusCode => StatusCodes.Status401Unauthorized;
        public override string Title => "Permisos o credenciales incorrectas";

        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, object? extencions) : base(message, extencions) { }
    }
}
