using AutoMapper;
using PPTBoardAPI.Authentication;
using PPTBoardAPI.DTOs;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.Helpers
{
    public class AutoMapProfiles : Profile
    {

        public AutoMapProfiles()
        {

            CreateMap<SpecialityCreationDTO, Speciality>()
                .ForMember(x => x.SpecialityDiscipline, options => options.MapFrom(MapCreationDtoSpecialityDiscipline));
            CreateMap<Speciality, SpecialityDTO>()
                .ForMember(x => x.Disciplines, options => options.MapFrom(MapSpecialitiesDisciplines));

            CreateMap<StudentDTO, Student>().ReverseMap();
            CreateMap<StudentViewDTO, Student>().ReverseMap();
            CreateMap<StudentCreationDTO, Student>();

            CreateMap<GroupDTO, Group>();
            CreateMap<Group, GroupDTO>().ForMember(g => g.Person, options => options.MapFrom(MapPersonUserView));
            CreateMap<GroupCreationDTO, Group>();
            CreateMap<Group, GroupIndexDTO>();

            CreateMap<DisciplineDTO, Discipline>().ReverseMap();
            CreateMap<DisciplineCreationDTO, Discipline>();

            CreateMap<AttendanceDTO, Attendance>().ReverseMap();
            CreateMap<AttendanceCreationDTO, Attendance>();

            CreateMap<ControllRecordCreationDTO, ControllRecord>();

            CreateMap<ControllTypeDTO, ControllType>().ReverseMap();

            CreateMap<Person, UserDTO>();

        }

        private UserViewDTO MapPersonUserView(Group group, GroupDTO groupDTO)
        {
            if (group.Person is null)
                return new UserViewDTO { Id = String.Empty, Fio = String.Empty };
            var person = new UserViewDTO
            {
                Id = group.Person.Id,
                Fio = group.Person.Fio
            };
            return person;
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
