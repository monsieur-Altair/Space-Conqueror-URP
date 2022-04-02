using _Application.Scripts.Infrastructure.Services;

namespace _Application.Scripts.Control
{
    public interface IInputService : IService
    {
        void HandleInput();
        void Init(BuildingsController buildingsController, SkillController skillController);
        void Refresh();
        void Reload();
    }
}