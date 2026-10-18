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
            CreateMap<Department, DepartmentUpdateDTO>().ReverseMap();
            



            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryUpdateDTO>().ReverseMap();



        }
    }
}
