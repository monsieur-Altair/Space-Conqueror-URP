using _Application.Scripts.Infrastructure.Services;

namespace _Application.Scripts.Control
{
    public interface IInputService : IService
    {
        void HandleInput();
        void Init(PlanetController planetController, SkillController skillController);
        void Refresh();
    }
}