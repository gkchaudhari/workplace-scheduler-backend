namespace workplace_scheduler.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool AgreeToTerms { get; set; }
        // Role: "Admin" or "Employee"
        public string Role { get; set; } = "Employee";
        public DateTime CreatedAt { get; set; }
    }
}
