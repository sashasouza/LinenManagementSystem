using System.ComponentModel.DataAnnotations;

namespace LinenManagement.Models
{
    public class CartLogDetail
    {
        [Key]
        public int CartLogDetailId { get; set; }
        public int CartLogId { get; set; }
        public int LinenId { get; set; }
        public int Count { get; set; }
        public Linen Linen { get; set; }
    }
}
