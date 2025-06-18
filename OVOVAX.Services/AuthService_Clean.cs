using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OVOVAX.Core.DTOs.Auth;
using OVOVAX.Core.Entities.Identity;
using OVOVAX.Core.Services.Contract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OVOVAX.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public AuthService(IConfiguration configuration, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailService = emailService;
        }


        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if email already exists
                if (await CheckEmailExistsAsync(request.Email))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Error = "Email already exists"
                    };
                }

                // Create new user
                var user = new AppUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName = request.Email.Split("@")[0],
                    NormalizedEmail = request.Email.ToUpper(),
                    NormalizedUserName = request.Email.Split("@")[0].ToUpper()
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Error = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                // Update last login
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Create token
                var token = await CreateTokenAsync(user);

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? string.Empty,
                        Token = token,
                        LastLogin = user.LastLogin
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Error = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Error = "Invalid email or password"
                    };
                }

                var result = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!result)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Error = "Invalid email or password"
                    };
                }

                // Update last login
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Create token
                var token = await CreateTokenAsync(user);

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? string.Empty,
                        Token = token,
                        LastLogin = user.LastLogin
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Error = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Don't reveal that user doesn't exist for security
                    return new ForgotPasswordResponse
                    {
                        Success = true,
                        Message = "If your email exists in our system, you will receive a password reset link."
                    };
                }

                // Generate password reset token
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // Create reset link for frontend React app
                var resetLink = $"http://localhost:5173/#/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}";

                // Send email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(email, resetLink);

                if (!emailSent)
                {
                    return new ForgotPasswordResponse
                    {
                        Success = false,
                        Error = "Failed to send reset email. Please try again later."
                    };
                }

                return new ForgotPasswordResponse
                {
                    Success = true,
                    Message = "Password reset email sent successfully"
                };
            }
            catch (Exception ex)
            {
                return new ForgotPasswordResponse
                {
                    Success = false,
                    Error = $"Failed to process request: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
                var key = Encoding.UTF8.GetBytes(secretKey);

                var validAudience = _configuration["JWT:ValidAudience"] ?? throw new InvalidOperationException("JWT ValidAudience not configured");
                var validIssuer = _configuration["JWT:ValidIssuer"] ?? throw new InvalidOperationException("JWT ValidIssuer not configured");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = validIssuer,
                    ValidateAudience = true,
                    ValidAudience = validAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);                var jwtToken = (JwtSecurityToken)validatedToken;
                
                // Use ClaimTypes.NameIdentifier instead of "nameid" to handle full URI claims
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Error = "User ID not found in token"
                    };
                }
                
                var userId = userIdClaim.Value;

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Error = "User not found"
                    };
                }

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? string.Empty,
                        Token = token,
                        LastLogin = user.LastLogin
                    }
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new AuthResponse
                {
                    Success = false,
                    Error = "Token has expired"
                };
            }
            catch (SecurityTokenValidationException)
            {
                return new AuthResponse
                {
                    Success = false,
                    Error = "Invalid token"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Error = $"Token validation failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            // Private Claims (user-defined)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Get Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var validAudience = _configuration["JWT:ValidAudience"] ?? throw new InvalidOperationException("JWT ValidAudience not configured");
            var validIssuer = _configuration["JWT:ValidIssuer"] ?? throw new InvalidOperationException("JWT ValidIssuer not configured");
            var durationInDays = _configuration["JWT:DurationInDays"] ?? throw new InvalidOperationException("JWT DurationInDays not configured");

            var token = new JwtSecurityToken(
                audience: validAudience,
                issuer: validIssuer,
                expires: DateTime.UtcNow.AddDays(double.Parse(durationInDays)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public PasswordValidationResult ValidatePassword(string password)
        {
            return PasswordValidator.ValidatePassword(password);
        }
    }
}
