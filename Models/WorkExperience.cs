using System;

namespace KNQARecruitmentPortal.Models
{
    public class WorkExperience
    {
        public int ExperienceId { get; set; }
        public string ApplicantUserId { get; set; } = "MOCK-APPLICANT-ID";
        public string CompanyName { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public DateTime? StartDate { get; set; } = DateTime.Now.AddYears(-2);
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public string CoreResponsibilities { get; set; } = "";
        public double CalculatedYears => EndDate.HasValue && StartDate.HasValue
            ? Math.Round((EndDate.Value - StartDate.Value).TotalDays / 365.25, 1)
            : 0;
    }
}
