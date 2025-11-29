using System.ComponentModel.DataAnnotations;

namespace LinenManagement.Models
{
    public class Carts
    {
        [Key]
        public int CartId { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public string? Type { get; set; }
    }
}
