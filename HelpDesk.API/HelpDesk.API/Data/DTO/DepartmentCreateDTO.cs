using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.Data.DTO
{
    public class DepartmentCreateDTO
    {
        [Required]
        public string Title { get; set; }
    }
}
