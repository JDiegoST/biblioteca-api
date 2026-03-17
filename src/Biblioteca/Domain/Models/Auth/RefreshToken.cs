using Biblioteca.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Domain.Models.Auth
{
    public class RefreshToken : ISoftDeleted
    {
        public Guid Id { get; set; }

        public string TokenHash { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public Guid? ReplacedByTokenId { get; set; }

        public string? CreatedByIp { get; set; }

        public Guid IdentityUserId { get; set; }
        public AplicationUser IdentityUser { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt && RevokedAt is not null;

        public DateTime? DeletedAt { get; set; }
    }
}
