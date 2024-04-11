using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class AuthorizationInfo
    {
        public string Email { get; set; } = null!;
        public string? EmailConfirmToken { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;

        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public virtual Guid UserId { get; set; }
    }
}
