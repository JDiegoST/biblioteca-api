namespace Biblioteca.Domain.Interfaces
{
    public interface ISoftDeleted
    {
        DateTime? DeletedAt { get; set; }
    }
}
