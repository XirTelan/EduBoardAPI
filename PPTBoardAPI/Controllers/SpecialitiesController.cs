using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.Controllers
{
    [Route("api/specialities")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SpecialitiesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public SpecialitiesController(ILogger<SpecialitiesController> logger, ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public  async Task<ActionResult<List<Speciality>>> GetSpecialities()
        {
            return await context.Specialities.ToListAsync();

        }
        [HttpGet("{Id:int}")]
        public ActionResult<Speciality> GetSpecialitiesById(int Id)
        {
            return NoContent();

        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Speciality speciality) {
            context.Specialities.Add(speciality);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete]
        public ActionResult Delete() {
            return NoContent();
        }
        [HttpPut]
        public ActionResult Put() {
            return NoContent();
        }
    }
}
