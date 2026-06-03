using DevHub.Models;
namespace DevHub.ViewModels.Candidate
{
    public class CandidateProfileViewModel
    {
        public DevHub.Models.Candidate CandidateInfo { get; set; } = null!;
        public int ProfileCompletion { get; set; }
        public Cv? Cv { get; set; }
        public List<CommonTechnology> AllTechnologies { get; set; } = new List<CommonTechnology>();
    }
}
