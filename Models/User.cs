namespace KNQARecruitmentPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? OtherName { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public byte Status { get; set; } = 1;
        public System.DateTime CreatedAt { get; set; }

        public Role? Role { get; set; }
    }
}