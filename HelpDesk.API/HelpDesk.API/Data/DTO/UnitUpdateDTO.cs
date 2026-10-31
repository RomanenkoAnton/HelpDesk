namespace HelpDesk.API.Data.DTO
{
    public class UnitUpdateDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SerialNumber { get; set; }
        public int CategoryId { get; set; }
        public int DepartmentId { get; set; }
    }
}
