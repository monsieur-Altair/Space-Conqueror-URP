using _Application.Scripts.Infrastructure.Services;

namespace _Application.Scripts.Control
{
    public interface IInputService : IService
    {
        public BuildingsController BuildingsController { get; }
        ISkillController SkillController { get; }
        void HandleInput();
        void Init(BuildingsController buildingsController, ISkillController skillController);
        void Refresh();
        void Reload();
    }
}