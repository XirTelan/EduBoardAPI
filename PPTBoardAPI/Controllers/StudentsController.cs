﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;
using PPTBoardAPI.Helpers;

namespace PPTBoardAPI.Controllers
{
    [Route("api/students")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public StudentsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Students.Include(s => s.Group).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var students = await queryable.OrderBy(x => x.SecondName).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<StudentDTO>>(students);

        }
        [HttpGet("getall")]
        public async Task<ActionResult<List<StudentDTO>>> Get()
        {
            var queryable = context.Students.Include(s => s.Group).AsQueryable();
            var students = await queryable.OrderBy(x => x.SecondName).ToListAsync();
            return mapper.Map<List<StudentDTO>>(students);

        }
        [HttpGet("filter")]
        public async Task<ActionResult<List<StudentDTO>>> FilterByName([FromQuery] PaginationDTO paginationDTO, [FromQuery] string query)
        {
            var queryable = context.Students.Where(s => s.SecondName.Contains(query)).Include(s => s.Group).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var students = await queryable.OrderBy(x => x.SecondName).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<StudentDTO>>(students);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDTO>> GetById(int id)
        {
            Student? student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return NotFound();
            else return mapper.Map<StudentDTO>(student);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Post([FromBody] StudentCreationDTO studentCreationDTO)
        {
            context.Students.Add(mapper.Map<Student>(studentCreationDTO));
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("excel")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> PostFromExcel([FromBody] List<StudentsExcelCreationDTO> studentsExcelCreationDTOs)
        {
            List<Student> students = new();
            foreach (StudentsExcelCreationDTO creationDTO in studentsExcelCreationDTOs)
            {
                int groupId = context.Groups.Where(g => g.Name.ToUpper() == creationDTO.GroupName.ToUpper()).Select(g => g.Id).FirstOrDefault();
                if (groupId == 0)
                {
                    context.Groups.Add(new Group { Name = creationDTO.GroupName });
                    await context.SaveChangesAsync();
                    groupId = context.Groups.Where(g => g.Name.ToUpper() == creationDTO.GroupName.ToUpper()).Select(g => g.Id).FirstOrDefault();
                }
                Student student = new()
                {
                    FirstName = creationDTO.FirstName,
                    SecondName = creationDTO.SecondName,
                    MiddleName = creationDTO.MiddleName,
                    GroupId = groupId,
                };
                students.Add(student);
            }
            context.Students.AddRange(students);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Put(int id, [FromBody] StudentCreationDTO studentDTO)
        {
            var student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return NotFound();
            student = mapper.Map(studentDTO, student);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            Student? student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return NotFound();
            context.Students.Remove(student);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
