using Biblioteca.Domain.Interfaces;

namespace Biblioteca.Domain.Models.Common
{
    public abstract class BaseModel : ISoftDeleted
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
