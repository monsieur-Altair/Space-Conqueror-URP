
namespace _Application.Scripts.Control
{
    public abstract class BaseInput:IInputService
    {
        protected abstract void AffectToPlanet();
        protected abstract void AffectToSkills();
        
        protected PlanetController _planetController;
        protected SkillController _skillController;

        public abstract void HandleInput();

        public void Init(PlanetController planetController, SkillController skillController)
        {
            _planetController = planetController;
            _skillController = skillController;
        }

        public void Refresh()
        {
            _skillController.RefreshSkills();
            _skillController.ReloadSkills();
        }
    }
}