using AutoMapper;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;

namespace HelpDesk.API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Department, DepartmentCreateDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();

        }
    }
}
