using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Helpers;

namespace PPTBoardAPI.Controllers
{
    [Route("api/specialities")]
    [ApiController]
 //   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SpecialitiesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public SpecialitiesController(ILogger<SpecialitiesController> logger, ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public  async Task<ActionResult<List<SpecialityDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Specialities.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var specialities =  await queryable.OrderBy(x=>x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<SpecialityDTO>>(specialities);

        }
        [HttpGet("{Id:int}")]
        public ActionResult<Speciality> GetSpecialitiesById(int Id)
        {
            return NoContent();

        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SpecialityCreationDTO specialityCreationDTO) {
            var speciality = mapper.Map<Speciality>(specialityCreationDTO);
            context.Specialities.Add(speciality);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete]
        public ActionResult Delete() {
            return NoContent();
        }
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Speciality speciality)
        {
            context.Specialities.Add(speciality);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
