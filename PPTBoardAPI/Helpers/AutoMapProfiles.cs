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
            CreateMap<SpecialityCreationDTO, Speciality>();

            CreateMap<StudentDTO, Student>().ReverseMap();
            CreateMap<StudentCreationDTO, Student>();

            CreateMap<GroupDTO, Group>().ReverseMap();
            CreateMap<GroupCreationDTO, Group>();

        }
    }
}
