using Control;

namespace _Application.Scripts.Control
{
    internal interface IInputService
    {
        void HandleInput();
        void Init(PlanetController planetController, SkillController skillController);
        
    }
}