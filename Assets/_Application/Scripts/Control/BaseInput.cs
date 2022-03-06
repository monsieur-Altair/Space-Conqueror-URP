
namespace _Application.Scripts.Control
{
    public abstract class BaseInput:IInputService
    {
        protected PlanetController _planetController;
        protected SkillController _skillController;

        public abstract void HandleInput();
        public void Init(PlanetController planetController, SkillController skillController)
        {
            _planetController = planetController;
            _skillController = skillController;
        }

        protected abstract void AffectToPlanet();
        protected abstract void AffectToSkills();
    }
}