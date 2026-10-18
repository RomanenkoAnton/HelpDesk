using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.Data.DTO
{
    public class CategoryCreateDTO
    {
        [Required]
        public string Title { get; set; }
    }
}
