using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KNQARecruitmentPortal.Data;

namespace KNQARecruitmentPortal.Services
{
    public interface IApplicationService
    {
        Task SubmitReviewAsync(Guid jobAppId, Guid reviewerId, int score, string comments);
        Task FinalizeStatusAsync(Guid jobAppId, int targetStatus, string userRole);
        Task ScheduleInterviewAsync(Guid jobAppId, DateTime date, string location, Guid interviewerId);
    }

    public class ApplicationService(AppDbContext context) : IApplicationService
    {
        public async Task SubmitReviewAsync(Guid jobAppId, Guid reviewerId, int score, string comments)
        {
            var review = new ApplicationReview
            {
                Id = Guid.NewGuid(),
                JobApplicationId = jobAppId,
                ReviewerUserId = reviewerId,
                Score = score,
                Comments = comments,
                ReviewedAt = DateTime.Now
            };

            await context.ApplicationReviews.AddAsync(review);
            await context.SaveChangesAsync();
        }

        public async Task FinalizeStatusAsync(Guid jobAppId, int targetStatus, string userRole)
        {
            if (userRole != "HRAdmin" && userRole != "SuperAdmin")
            {
                throw new UnauthorizedAccessException("Security Violation: Only HRAdmin or SuperAdmin can change an application's final state.");
            }

            var app = await context.JobApplications.FirstOrDefaultAsync(x => x.Id == jobAppId);
            if (app != null)
            {
                app.Status = targetStatus; 
                context.JobApplications.Update(app);
                await context.SaveChangesAsync();
            }
        }

        public async Task ScheduleInterviewAsync(Guid jobAppId, DateTime date, string location, Guid interviewerId)
        {
            var app = await context.JobApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == jobAppId);

            if (app == null || app.Status != 1)
            {
                throw new InvalidOperationException("Validation Guard Fault: Only applications with an active 'Shortlisted' status can have an InterviewSlot created against them.");
            }

            var slot = new InterviewSlot
            {
                Id = Guid.NewGuid(),
                JobApplicationId = jobAppId,
                ScheduledAt = date,
                LocationOrLink = location,
                InterviewerUserId = interviewerId,
                Status = 0 
            };

            await context.InterviewSlots.AddAsync(slot);
            await context.SaveChangesAsync();
        }
    }
}
