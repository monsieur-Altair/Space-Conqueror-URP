using _Application.Scripts.SavedData;

namespace _Application.Scripts.Infrastructure.Services.Progress
{
    public interface IProgressReader
    {
        void ReadProgress(PlayerProgress playerProgress);
    }
}