using System.ComponentModel.DataAnnotations;

namespace LinenManagement.Models
{
    public class CartLog
    {
        [Key]
        public int CartLogId { get; set; }
        public string? ReceiptNumber { get; set; }
        public int? ReportedWeight { get; set; }
        public int ActualWeight { get; set; }
        public string? Comments { get; set; }
        public DateTime DateWeighed {  get; set; }
        public int CartId { get; set; }
        public int LocationId { get; set; }
        public int EmployeeId { get; set; }
        public Carts? Cart { get; set; }
        public Locations? Location { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<CartLogDetail>? Details { get; set; }
    }
}
