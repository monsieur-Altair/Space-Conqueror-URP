namespace _Application.Scripts.Control
{
    public interface IInputService
    {
        void HandleInput();
        void Init(PlanetController planetController, SkillController skillController);
        
    }
}