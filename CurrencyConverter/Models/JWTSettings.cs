using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Models
{
    public class JWTSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }

        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }

    }
    public class Login
    {
        [Required]
        
        public string GrantType { get; set; } 

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class Token
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; } = "bearer";
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    }
