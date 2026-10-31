namespace HelpDesk.API.Data.DTO
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentTitle { get; set; }

        public string Notes { get; set; }

        public DateTime? SetupDate { get; set; }
        public DateTime? SentToSZIDate { get; set; }
        public DateTime? ReturnedFromSZIDate { get; set; }
        public DateTime? IssuedDate { get; set; }

        public string AssignedEmployee { get; set; }
        public string IssuedTo { get; set; }
        public bool InvoiceSigned { get; set; }
    }

    public class TicketCreateDTO
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public int DepartmentId { get; set; }
        public string Notes { get; set; }

        public DateTime? SetupDate { get; set; }
        public DateTime? SentToSZIDate { get; set; }
        public DateTime? ReturnedFromSZIDate { get; set; }
        public DateTime? IssuedDate { get; set; }

        public string AssignedEmployee { get; set; }
        public string IssuedTo { get; set; }
        public bool InvoiceSigned { get; set; }
    }

    public class TicketUpdateDTO : TicketCreateDTO
    {
        public int Id { get; set; }
    }
}