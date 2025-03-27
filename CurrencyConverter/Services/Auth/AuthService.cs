using CurrencyConverter.IServices.Auth;
using CurrencyConverter.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CurrencyConverter.Common.Resources;

namespace CurrencyConverter.Services.Auth
{
    public class UnsupportedGrantTypeException : Exception
    {

        public UnsupportedGrantTypeException(string name)
             : base(String.Format("Invalid Grant: {0}", name))
        {
        }
    }
    public class AuthService : IAuthService
    {
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = configuration.GetSection("JWTSettings:SecretKey").Value;
        }
        public async Task<ServiceResponse<Token>> Authenticate(Login loginModel)
        {
            

            if (loginModel.GrantType != "password")
            {
                throw new UnsupportedGrantTypeException(loginModel.GrantType);
            }

            //Verify and Fetch logged in user details from database. hardcoding now.
            ServiceResponse<Token> serviceResponse = new ServiceResponse<Token>();
            
            
            var userlist = new List<(string userId,string email, string userName, string password, string role)>
            {
                ("1000","test@gmail.com", "Test", "Test123", "User"),
                ("1001","card@gmail.com", "Card", "CardTest", "Admin")
            };
            var user = userlist.FirstOrDefault(u => u.userName == loginModel.Username && u.password == loginModel.Password);
            if (user == default)
            {
                return null;
            }
              
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = Encoding.ASCII.GetBytes(_secretKey);
                var expires = GetExpiry();
                // var siteIds = Array.ConvertAll(employee.Result.SitesAssigned.Split(","), int.Parse);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                    new Claim("clientId", user.userId),
                    new Claim("Email", user.email),
                    new Claim("role", user.role)
                  //Fetch loggedin user details from database and assign to claim
                }),
                    Expires = expires,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(token);

                //  Insert token details to database.
                //await _authenticationUserRepository.InsertToken(employee.Result.id, accessToken, expires);
            //}
            serviceResponse.Result = new Token()
            {
                AccessToken = accessToken,
                TokenType = "bearer",
                UserId =Convert.ToInt32( user.userId),
                ExpiresAt = expires,
                CreatedAt = DateTime.Now,

            };

            return serviceResponse;
        }

        private DateTime GetExpiry()
        {
            DateTime expiry;
            string ExpiryType = _configuration.GetSection("JWTSettings:ExpiryType")?.Value;
            string ExpiryDuration = _configuration.GetSection("JWTSettings:ExpiryDuration")?.Value;

            if (ExpiryType == "Hours")
            {
                if (string.IsNullOrEmpty(ExpiryDuration))
                    expiry = DateTime.UtcNow.AddHours(24); // Default Hour
                else
                    expiry = DateTime.UtcNow.AddHours(Convert.ToInt32(ExpiryDuration));
            }
            else if (ExpiryType == "Days")
            {
                if (string.IsNullOrEmpty(ExpiryDuration))
                    expiry = DateTime.UtcNow.AddDays(1); // Default Day
                else
                    expiry = DateTime.UtcNow.AddDays(Convert.ToInt32(ExpiryDuration));
            }
            else
            {
                if (string.IsNullOrEmpty(ExpiryDuration))
                    expiry = DateTime.UtcNow.AddMinutes(1440);  // Default Minutes
                else
                    expiry = DateTime.UtcNow.AddMinutes(Convert.ToInt32(ExpiryDuration));
            }
            return expiry;
        }
    }
}
