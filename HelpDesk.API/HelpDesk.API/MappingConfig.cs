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

            CreateMap<Unit, UnitDTO>()
                .ForMember(d => d.CategoryTitle, o => o.MapFrom(s => s.Category.Title))
                .ForMember(d => d.DepartmentTitle, o => o.MapFrom(s => s.Department.Title));

            CreateMap<UnitCreateDTO, Unit>();
            CreateMap<UnitUpdateDTO, Unit>();
            CreateMap<Ticket, TicketDTO>()
    .ForMember(d => d.DepartmentTitle, o => o.MapFrom(s => s.Department.Title));

            CreateMap<TicketCreateDTO, Ticket>();
            CreateMap<TicketUpdateDTO, Ticket>();



        }
    }
}
