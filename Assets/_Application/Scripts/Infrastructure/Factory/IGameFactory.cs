using _Application.Scripts.Infrastructure.Services;

namespace _Application.Scripts.Infrastructure.Factory
{
    public interface IGameFactory : IService
    {
        void CreateWorld();
    }
}