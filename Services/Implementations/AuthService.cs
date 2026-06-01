using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserAccountRepository _userRepo;
    private readonly ICandidateRepository   _candidateRepo;
    private readonly IRecruiterRepository   _recruiterRepo;

    public AuthService(
        IUserAccountRepository userRepo,
        ICandidateRepository   candidateRepo,
        IRecruiterRepository   recruiterRepo)
    {
        _userRepo      = userRepo;
        _candidateRepo = candidateRepo;
        _recruiterRepo = recruiterRepo;
    }

    public Task<UserAccount?> FindUserByEmailAsync(string email)
        => _userRepo.GetByEmailAsync(email);

    public Task<UserAccount?> FindUserByGoogleIdAsync(string googleId)
        => _userRepo.GetByGoogleIdAsync(googleId);

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(hash) || hash == "GOOGLE_OAUTH") return false;
        try { return BCrypt.Net.BCrypt.Verify(password, hash); }
        catch { return false; }
    }

    public async Task<UserAccount> CreateCandidateGoogleAccountAsync(
        string email, string googleId, string? fullName, string? avatarUrl)
    {
        var user = await _userRepo.AddAsync(new UserAccount
        {
            Email        = email,
            PasswordHash = "GOOGLE_OAUTH",
            GoogleId     = googleId,
            UserType     = "Candidate",
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow,
            LastUpdated  = DateTime.UtcNow
        });

        var candidate = await _candidateRepo.AddAsync(new Candidate
        {
            CandidateId = user.UserId,
            FullName    = fullName ?? email,
            ImageUrl    = avatarUrl
        });

        user.Candidate = candidate;
        return user;
    }

    public async Task<UserAccount> CreateRecruiterGoogleAccountAsync(
        string email, string googleId, string? fullName, string? avatarUrl, string? phone)
    {
        var user = await _userRepo.AddAsync(new UserAccount
        {
            Email        = email,
            PasswordHash = "GOOGLE_OAUTH",
            GoogleId     = googleId,
            UserType     = "Recruiter",
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow,
            LastUpdated  = DateTime.UtcNow
        });

        var recruiter = await _recruiterRepo.AddAsync(new Recruiter
        {
            RecruiterId    = user.UserId,
            FullName       = fullName ?? email,
            CompanyName    = fullName ?? email,
            Phone          = phone,
            CompanyLogoUrl = avatarUrl
        });

        user.Recruiter = recruiter;
        return user;
    }

    public Task UpdateLastLoginAsync(int userId)
        => _userRepo.UpdateLastLoginAsync(userId);

    public Task LinkGoogleIdAsync(int userId, string googleId)
        => _userRepo.UpdateGoogleIdAsync(userId, googleId);

    public Task SyncCandidateAvatarAsync(int candidateId, string avatarUrl)
        => _candidateRepo.UpdateAvatarAsync(candidateId, avatarUrl);

    public Task SyncRecruiterAvatarAsync(int recruiterId, string avatarUrl)
        => _recruiterRepo.UpdateCompanyLogoAsync(recruiterId, avatarUrl);

    public Task UpdatePasswordAsync(int userId, string passwordHash)
        => _userRepo.UpdatePasswordAsync(userId, passwordHash);
}
