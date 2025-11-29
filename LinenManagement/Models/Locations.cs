using System.ComponentModel.DataAnnotations;

namespace LinenManagement.Models
{
    public class Locations
    {
        [Key]
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
