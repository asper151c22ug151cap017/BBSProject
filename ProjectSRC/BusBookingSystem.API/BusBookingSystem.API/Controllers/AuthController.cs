// ============================================================================
// Project Name : BusBookingSystem
// File Name    : AuthController.cs
// Created By   : Kaviraj M
// Created On   : 19/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Manages authentication and JWT token generation for users 
//                in the Bus Booking System. Handles user login, validation, 
//                and secure token issuance.
// ============================================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusBookingSystem.Application;
using BusBookingSystem.Application.login;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EventPlans.API.Controllers
{

    /// <summary>
    /// Handles authentication-related endpoints for the Bus Booking System.
    /// Provides functionality for user login and JWT token generation.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;

        private readonly IBBSAuth _EmsAuth;
        private readonly ErrorHandler _errorHandling;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class 
        /// with configuration, authentication repository, and error handler dependencies.
        /// </summary>
        /// <param name="config">Application configuration for JWT settings.</param>
        /// <param name="authRepo">Authentication repository instance.</param>
        /// <param name="errorHandler">Error handler for logging exceptions.</param>

        public AuthController(IConfiguration config, IBBSAuth Authconsole, ErrorHandler bBSError)
        {
            _config = config;
            _EmsAuth = Authconsole;
            _errorHandling = bBSError;

        }


        // --------------------------------------------------------------------
        // ✅ USER LOGIN & JWT TOKEN GENERATION
        // --------------------------------------------------------------------
        /// <summary>
        /// Authenticates the user and issues a JWT token if credentials are valid.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>
        /// Returns a JWT token upon successful authentication, 
        /// or an unauthorized response if login fails.
        /// </returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] RequestloginDto request)
        {
            try
            {
                var user = _EmsAuth.Login(request.Email, request.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid Email or password" });

                // JWT settings
                var jwtSettings = _config.GetSection("Jwt");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim("userid", user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresMinutes"])),
                    signingCredentials: creds
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            catch (Exception ex)
            {
                _errorHandling.Capture(ex, "An Error while Authorized User");
                return Unauthorized(new { message = "An error occurred during login.", error = ex.Message });
            }
        }

    }

}



