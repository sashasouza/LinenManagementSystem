namespace LinenManagement.Models
{
    public class CartLogDTO
    {
        public int CartLogId { get; set; }
        public string? ReceiptNumber { get; set; }
        public int? ReportedWeight { get; set; }
        public int ActualWeight { get; set; }
        public string? Comments { get; set; }
        public DateTime DateWeighed { get; set; }
        public object Cart { get; set; }     
        public object Location { get; set; }
        public object Employee { get; set; }
        public List<CartLogLinenDTO> Linen { get; set; }
    }
}
