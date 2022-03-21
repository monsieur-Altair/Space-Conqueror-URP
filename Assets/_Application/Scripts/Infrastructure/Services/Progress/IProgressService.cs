using _Application.Scripts.SavedData;

namespace _Application.Scripts.Infrastructure.Services.Progress
{
    public interface IProgressService : IService
    {
        PlayerProgress PlayerProgress { get; set; }
    }
}