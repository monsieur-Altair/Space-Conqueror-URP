using _Application.Scripts.SavedData;

namespace _Application.Scripts.Infrastructure.Services.Progress
{
    public class ProgressService : IProgressService
    {
        public PlayerProgress PlayerProgress { get; set; }
    }
}