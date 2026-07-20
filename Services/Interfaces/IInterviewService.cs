using System;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IInterviewService
{
    Task<Interview> CreateInterviewAsync(int recruiterId, int applicationId, DateTime scheduledTime, string interviewType, string locationOrLink, string? notes);
    Task<Interview> UpdateInterviewAsync(int recruiterId, int interviewId, DateTime scheduledTime, string interviewType, string locationOrLink, string? notes, string? reasonForChange = null);
    Task<bool> UpdateStatusAsync(int recruiterId, int interviewId, string status, string? reason = null);
    Task SyncInterviewStatusesAsync();
}
