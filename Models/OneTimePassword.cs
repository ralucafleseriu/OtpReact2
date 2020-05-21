using System;

namespace OtpReact.Models
{
    public class OneTimePassword
    {
        public DateTime ExpirationDateTime { get; set; }

        public Guid UserId { get; set; }

        public string Password { get; set; }
        public OneTimePassword(Guid userId, string password, DateTime expirationDateTime)
        {
            UserId = userId;
            Password = password;
            ExpirationDateTime = expirationDateTime;
        }
    }
}
