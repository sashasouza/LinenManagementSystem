namespace LinenManagement.Models
{
    public class RefreshRequest
    {
        public int EmployeeId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
