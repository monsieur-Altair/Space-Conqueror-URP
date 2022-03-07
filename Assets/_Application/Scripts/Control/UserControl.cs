using UnityEngine;

namespace _Application.Scripts.Control
{
    public class UserControl : MonoBehaviour
    {
        private bool _isActive;
        public static UserControl Instance;

        private IInputService _inputService;

        public void Awake()
        {
            if (Instance==null) 
                Instance = this;

            //_inputService = Game.InputService;
            //_inputService.Init(new PlanetController(Camera.main), GetComponent<SkillController>());
            _inputService = RegisterInputService();
        }

        public void Update()
        {
            if(!_isActive)
                return;
            
            _inputService.HandleInput();
        }

        public void Disable()
        {
            _isActive = false;
            _inputService.Disable();
        }
        
        public void Enable()
        {
            _isActive = true;
            _inputService.Enable();
        }

        private IInputService RegisterInputService()
        {
            IInputService service;
            if (Application.isEditor)
                service = new StandaloneInput();
            else
                service = new MobileInput();
            service.Init(new PlanetController(Camera.main),GetComponent<SkillController>());
            return service;
        }
    }
}


