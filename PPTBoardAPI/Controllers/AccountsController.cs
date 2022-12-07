using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PPTBoardAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : ControllerBase
    {
        private readonly UserManager<Person> userManager;
        private readonly SignInManager<Person> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AccountsController(UserManager<Person> userManager, SignInManager<Person> signInManager, IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
            List<UserDTO> userDTOs = new List<UserDTO>();
            foreach (Person user in users)
            {
                string role = "";
                var claims = await userManager.GetClaimsAsync(user);
                Claim? claim = claims.FirstOrDefault(c => c.Type == "type");
                if (claim != null)
                    role = claim.Value;

                userDTOs.Add(new UserDTO { Id = user.Id, UserName = user.UserName, Fio = user.Fio, Role = role });
            }

            return userDTOs;
        }
        [HttpPost("role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]

        public async Task<ActionResult> ChangeRole([FromBody] UserRoleDTO userRoleDTO)
        {
            var user = await userManager.FindByIdAsync(userRoleDTO.UserId);
            var claims = await userManager.GetClaimsAsync(user);
            var claim = claims.FirstOrDefault(c => c.Type == "type");
            if (claim != null)
            {
                if (claim.Value == userRoleDTO.Role)
                    return NoContent();
                await userManager.RemoveClaimAsync(user, claim);
            }
            await userManager.AddClaimAsync(user, new Claim("type", userRoleDTO.Role));
            return NoContent();
        }

        [HttpPost("create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]

        public async Task<ActionResult> Create([FromBody] UserRegisterCredentials userRegisterCredentials)
        {
            var user = new Person() { UserName = userRegisterCredentials.UserName, Fio = userRegisterCredentials.Fio };
            var result = await userManager.CreateAsync(user, userRegisterCredentials.Password);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
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
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.UserName, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return await BuildToken(userCredentials.UserName);
            }
            else
            {
                return BadRequest("Incorrect");
            }
        }

        private async Task<AuthenticationResponse> BuildToken(string userName)
        {
            var claims = new List<Claim>()
            {
                new Claim("name",userName)
            };
            var user = await userManager.FindByNameAsync(userName);
            var claimDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(1);
            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);
            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

    }
}
