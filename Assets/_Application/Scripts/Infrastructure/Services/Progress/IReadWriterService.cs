using _Application.Scripts.SavedData;

namespace _Application.Scripts.Infrastructure.Services.Progress
{
    public interface IReadWriterService : IService
    {
        PlayerProgress ReadProgress();
        void WriteProgress();
    }
}