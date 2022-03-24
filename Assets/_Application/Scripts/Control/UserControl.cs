using UnityEngine;

namespace _Application.Scripts.Control
{
    public class UserControl : MonoBehaviour
    {
        private bool _isActive;

        private IInputService _inputService;

        public void Init(BuildingsController buildingsController ,SkillController skillController)
        {
            //_inputService = Game.InputService;
            //_inputService.Init(new BuildingsController(Camera.main), GetComponent<SkillController>());
            _inputService = RegisterInputService(buildingsController ,skillController);
            //_inputService = AllServices.Instance.Single<IInputService>();
        }

        public void Update()
        {
            if(!_isActive)
                return;
            
            _inputService.HandleInput();
        }

        public void Disable() => 
            _isActive = false;

        public void Enable()
        {
            _isActive = true;
            _inputService.Refresh();
        }

        private static IInputService RegisterInputService(BuildingsController buildingsController ,SkillController skillController)
        {
            IInputService service;
            if (Application.isEditor)
                service = new StandaloneInput();
            else
                service = new MobileInput();
            service.Init(buildingsController, skillController);
            return service;
        }
    }
}


