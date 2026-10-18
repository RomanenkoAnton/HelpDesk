namespace HelpDesk.API.Models
{
    public class Unit
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SerialNumber { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }
    }
}
