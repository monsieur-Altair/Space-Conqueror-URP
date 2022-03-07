
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

        public void Enable()
        {
            _skillController.Enable();
        }

        public void Disable()
        {
            _skillController.Disable();
        }
    }
}