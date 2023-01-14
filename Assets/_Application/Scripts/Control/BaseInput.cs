
namespace _Application.Scripts.Control
{
    public abstract class BaseInput : IInputService
    {
        protected abstract void AffectToBuilding();
        protected abstract void AffectToSkills();
        
        public BuildingsController BuildingsController { get; private set; }
        public SkillController SkillController { get; private set; }

        public abstract void HandleInput();

        public void Init(BuildingsController buildingsController, SkillController skillController)
        {
            BuildingsController = buildingsController;
            SkillController = skillController;
        }

        public void Refresh() => 
            SkillController.RefreshSkills();

        public void Reload() => 
            SkillController.ReloadSkills();
    }
}