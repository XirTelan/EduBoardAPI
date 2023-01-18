using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PPTBoardAPI.Authentication;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : ControllerBase
    {
        private readonly UserManager<Person> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<Person> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AccountsController(UserManager<Person> userManager, SignInManager<Person> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<List<UserDTO>>> GetUserList([FromQuery] PaginationDTO paginationDTO)
        {
            IQueryable<Person> queryable = context.Users.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            List<Person> users = await queryable.OrderBy(u => u.UserName).Paginate(paginationDTO).ToListAsync();
            List<UserDTO> userDTOs = new();
            foreach (Person user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userDTOs.Add(new UserDTO { Id = user.Id, UserName = user.UserName, Fio = user.Fio, Roles = roles.ToList() });
            }

            return userDTOs;
        }
        [HttpGet("getall")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "HasWritePerm")]
        public async Task<ActionResult<List<UserViewDTO>>> GetAllUsers()
        {
            IQueryable<Person> queryable = context.Users.AsQueryable();
            List<Person> users = await queryable.OrderBy(u => u.UserName).ToListAsync();
            List<UserViewDTO> userDTOs = new();
            foreach (Person user in users)
            {
                userDTOs.Add(new UserViewDTO { Id = user.Id, Fio = user.Fio });
            }

            return userDTOs;
        }
        [HttpPost("role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]

        public async Task<ActionResult> ChangeRole([FromBody] UserRoleDTO userRoleDTO)
        {
            var user = await userManager.FindByIdAsync(userRoleDTO.UserId);
            await RemoveRolesFromUser(user);
            await AddRolesToUser(user, new List<string> { userRoleDTO.Role });
            return NoContent();
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterCredentials model)
        {
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            Person user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Fio = model.Fio,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            await AddRolesToUser(user, new List<string> { UserRoles.User });
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRegisterCredentials model)
        {
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            Person user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Fio = model.Fio,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string messageError = String.Empty;
                if (result.Errors.Any())
                    foreach (var error in result.Errors)
                        messageError += error.Description + Environment.NewLine;
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = messageError });
            }
            await AddRolesToUser(user, new List<string> { UserRoles.User, UserRoles.Admin });
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }


        async private Task AddRolesToUser(Person user, List<string> roles)
        {
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

                if (await roleManager.RoleExistsAsync(role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
        async private Task RemoveRolesFromUser(Person user)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (string role in userRoles)
            {
                if (await roleManager.RoleExistsAsync(role) && role != UserRoles.User)
                {
                    await userManager.RemoveFromRoleAsync(user, role);
                }
            }

        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<AuthenticationResponse>> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);

            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.UserName, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);
            var user = await userManager.FindByNameAsync(userCredentials.UserName);
            if (result.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = CreateToken(authClaims);

                var refreshToken = GenerateRefreshToken();
                _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await userManager.UpdateAsync(user);

                Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });
                Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });

                return Ok(new
                {
                    user.Fio,
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Roles = userRoles
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Неверный логин/пароль" });
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!(Request.Cookies.TryGetValue("X-Username", out var userName) && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)))
                return BadRequest();
            var user = userManager.Users.FirstOrDefault(p => p.UserName == userName && p.RefreshToken == refreshToken);
            if (user is null) return BadRequest();
            await RevokeToken(user);
            Response.Cookies.Delete("X-Username", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });
            Response.Cookies.Delete("X-Refresh-Token", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });
            return NoContent();
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            _ = int.TryParse(configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> RefreshToken()
        {

            if (!(Request.Cookies.TryGetValue("X-Username", out var userName) && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)))
                return BadRequest();

            var user = userManager.Users.FirstOrDefault(p => p.UserName == userName && p.RefreshToken == refreshToken);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token");
            }
            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var newAccessToken = CreateToken(authClaims);

            return new ObjectResult(new
            {
                user.Fio,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                Roles = userRoles
            });
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult passwordChangeResult = await userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);


        }

        [HttpPost]
        [Route("revoke/{username}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user is null) return BadRequest("Invalid user name");
            await RevokeToken(user);
            return NoContent();
        }


        [HttpPost]
        [Route("revoke-all")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                await RevokeToken(user);
            }

            return NoContent();
        }

        private async Task<ActionResult> RevokeToken(Person user)
        {
            user.RefreshToken = null;
            await userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}
