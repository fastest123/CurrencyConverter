using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Models;
using Microsoft.AspNetCore.Authorization;
using CurrencyConverter.IServices.Auth;

namespace CurrencyConverter.Controllers.Auth
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/auth")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController ( IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Generate JWT token to use with the API
        /// </summary>
        /// <remarks>
        ///   To get an access token using the password grant type, send a POST request to the /auth/token endpoint,
        ///   passing the application credentials in the request body.
        ///
        ///   Sample Request:
        ///
        ///     POST /v1/auth/token
        ///     {
        ///        "grantType": "password",
        ///        "username": "username",
        ///        "password": "password"
        ///     }
        ///
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<ActionResult<Token>> Login([FromBody] Login loginModel)
        {
            var response = await _authService.Authenticate(loginModel);

            if (response == null)
                return BadRequest(new { message = "Username or Password is incorrect" });
            return Ok(response.Result);
        }
    }
}
