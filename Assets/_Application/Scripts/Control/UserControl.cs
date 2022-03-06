using UnityEngine;

namespace _Application.Scripts.Control
{
    public class UserControl : MonoBehaviour
    {
        public bool isActive=false;
        public static UserControl Instance;

        private IInputService _inputService;

        public void Awake()
        {
            if (Instance==null) 
                Instance = this;

            _inputService = RegisterInputService();
            _inputService.Init(new PlanetController(Camera.main),GetComponent<SkillController>());
        }

        public void Update()
        {
            if(!isActive)
                return;
            
            _inputService.HandleInput();
        }

        private static IInputService RegisterInputService()
        {
            if (Application.isEditor)
                return new StandaloneInput();
            else
                return new MobileInput();
        }
    }
}


