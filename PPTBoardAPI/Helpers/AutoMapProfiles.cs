﻿using AutoMapper;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.Helpers
{
    public class AutoMapProfiles : Profile
    {
        public AutoMapProfiles()
        {
            CreateMap<SpecialityCreationDTO, Speciality>()
                .ForMember(x=>x.SpecialityDiscipline, options=>options.MapFrom(MapCreationDtoSpecialityDiscipline));
            CreateMap<Speciality, SpecialityDTO>()
                .ForMember(x => x.Disciplines, options => options.MapFrom(MapSpecialitiesDisciplines));

            CreateMap<StudentDTO, Student>().ReverseMap();
            CreateMap<StudentCreationDTO, Student>()
                .ForMember(x=>x.Group, options=> options.MapFrom(mapStudentGroup));

            CreateMap<GroupDTO, Group>().ReverseMap();
            CreateMap<GroupCreationDTO, Group>();

            CreateMap<DisciplineDTO, Discipline>().ReverseMap();
            CreateMap<DisciplineCreationDTO, Discipline>();

        }

        private List<DisciplineDTO> MapSpecialitiesDisciplines(Speciality speciality, SpecialityDTO specialityDTO)
        {
            var resultDiscplines = new List<DisciplineDTO>();
            if (speciality.SpecialityDiscipline != null)
            {
                foreach (SpecialityDiscipline discipline in speciality.SpecialityDiscipline)
                {
                    resultDiscplines.Add(new DisciplineDTO() { Id = discipline.DisciplineId, Name = discipline.Discipline.Name });
                }
            }

            return resultDiscplines;
        }


        private Group mapStudentGroup(StudentCreationDTO studentCreationDTO, Student student)
        {
            return new Group() { Id = studentCreationDTO.GroupId };
        }

        private List<SpecialityDiscipline> MapCreationDtoSpecialityDiscipline(SpecialityCreationDTO specialityCreationDTO, Speciality speciality)
        {
            var result = new List<SpecialityDiscipline>();
            if (specialityCreationDTO.DisciplineIds == null) return result;
            foreach (var id in specialityCreationDTO.DisciplineIds)
            {
                result.Add(new SpecialityDiscipline() { DisciplineId = id });
            }
            return result;
        }
    }
}
