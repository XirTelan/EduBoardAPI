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
        }
    }
}
