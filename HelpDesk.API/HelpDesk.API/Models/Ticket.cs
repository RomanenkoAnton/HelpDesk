namespace HelpDesk.API.Models
{
    public class Ticket
    {
        // 1. Первинний ключ (Primary Key)
        public int Id { get; set; }

        // Основні поля
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;

        // 2. Зовнішній ключ (Foreign Key)
        public int DepartmentId { get; set; }

        // 3. Навігаційна властивість (посилання на сам відділ)
        public Department Department { get; set; } = null!;

        // Додаткові поля та примітки
        public string? Notes { get; set; }

        // Дати (Nullable, бо можуть бути порожніми)
        public DateTime? SetupDate { get; set; }
        public DateTime? SentToSZIDate { get; set; }
        public DateTime? ReturnedFromSZIDate { get; set; }
        public DateTime? IssuedDate { get; set; }

        // Інші поля
        public string? AssignedEmployee { get; set; }
        public string? IssuedTo { get; set; }
        public bool InvoiceSigned { get; set; }
    }
}
