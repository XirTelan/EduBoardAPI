using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.Authentication;
using PPTBoardAPI.DTOs;

namespace PPTBoardAPI.Controllers
{

    [ApiController]
    [Route("api/controlltype")]
    public class ControllTypeController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ControllTypeController(UserManager<Person> userManager, SignInManager<Person> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<ControllTypeDTO>>> GetTypes()
        {
            var controllTypes = await context.ControllTypes.ToListAsync();
            return mapper.Map<List<ControllTypeDTO>>(controllTypes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ControllTypeDTO>> GetTypesById(int id)
        {
            var controllType = await context.ControllTypes.Where(ct => ct.Id == id).FirstOrDefaultAsync();
            if (controllType is null) return NotFound();
            else
            {
                return mapper.Map<ControllTypeDTO>(controllType);
            }
        }
    }
}
