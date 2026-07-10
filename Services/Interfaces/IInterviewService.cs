using System;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IInterviewService
{
    Task<Interview> CreateInterviewAsync(int applicationId, int recruiterId, int candidateId, DateTime scheduledTime, string interviewType, string? meetingLink, string? location, string? notes);
    Task<Interview> UpdateInterviewAsync(int interviewId, DateTime scheduledTime, string interviewType, string? meetingLink, string? location, string? notes);
}
