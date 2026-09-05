using Auth.Api.Entities;
using Auth.Api.Helpers;
using Auth.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<AppUser> userManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
        }



        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email Is Already Registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username Is Already Registered!" };


            var user = new AppUser()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username

            };

            var result = await _userManager.CreateAsync(user, model.password);
            if (!result.Succeeded)
            {
                var errors = new StringBuilder();

                foreach (var error in result.Errors)
                {
                    errors.AppendLine(error.Description);
                }
                return new AuthModel() { Message = errors.ToString() };
            }


            await _userManager.AddToRoleAsync(user, "User");

            var token = await CreateJwtToken(user);

            return new AuthModel()
            {
                Message = "User Registered Successfully!",
                Email = user.Email,
                IsAuthenticated = true,
                Username = user.UserName,
                Roles = new List<string> { "User" },
                ExpiresOn = token.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("Roles", role));
            }


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)

            }
            .Union(userClaims)
            .Union(roleClaims);



            var symmetricSecuritykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var siningCredentials = new SigningCredentials(symmetricSecuritykey, SecurityAlgorithms.HmacSha256);


            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: siningCredentials
                );
        }
    }
}
