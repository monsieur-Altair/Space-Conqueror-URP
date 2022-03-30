using _Application.Scripts.SavedData;

namespace _Application.Scripts.Infrastructure.Services.Progress
{
    public interface IProgressWriter
    {
        void WriteProgress(PlayerProgress playerProgress);
    }
}