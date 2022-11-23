using AutoMapper;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.Helpers
{
    public class AutoMapProfiles : Profile
    {
        public AutoMapProfiles()
        {
            CreateMap<SpecialityDTO, Speciality>().ReverseMap();
            CreateMap<SpecialityCreationDTO, Speciality>()
                .ForMember(x=>x.SpecialityDiscipline, options=>options.MapFrom(MapSpecialitiesDiscipline));

            CreateMap<StudentDTO, Student>().ReverseMap();
            CreateMap<StudentCreationDTO, Student>();

            CreateMap<GroupDTO, Group>().ReverseMap();
            CreateMap<GroupCreationDTO, Group>();

            CreateMap<DisciplineDTO, Discipline>().ReverseMap();
            CreateMap<DisciplineCreationDTO, Discipline>();

        }

        private List<SpecialityDiscipline> MapSpecialitiesDiscipline(SpecialityCreationDTO specialityCreationDTO, Speciality speciality)
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
