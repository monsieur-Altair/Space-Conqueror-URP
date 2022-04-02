
namespace _Application.Scripts.Control
{
    public abstract class BaseInput:IInputService
    {
        protected abstract void AffectToBuilding();
        protected abstract void AffectToSkills();
        
        protected BuildingsController _buildingsController;
        protected SkillController _skillController;

        public abstract void HandleInput();

        public void Init(BuildingsController buildingsController, SkillController skillController)
        {
            _buildingsController = buildingsController;
            _skillController = skillController;
        }

        public void Refresh() => 
            _skillController.RefreshSkills();

        public void Reload() => 
            _skillController.ReloadSkills();
    }
}