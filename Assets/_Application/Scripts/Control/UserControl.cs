using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public class UserControl : MonoBehaviourService
    {
        private bool _isActive;

        private IInputService _inputService;

        public override void Init()
        {
            base.Init();
            
            BuildingsController buildingsController = new BuildingsController(AllServices.Get<GlobalCamera>().MainCamera);
            _inputService = RegisterInputService(buildingsController, AllServices.Get<SkillController>());
        }

        public void Update()
        {
            if(!_isActive)
                return;
            
            _inputService.HandleInput();
        }

        public void Disable() => 
            _isActive = false;

        public void Enable() => 
            _isActive = true;

        public void Refresh() => 
            _inputService.Refresh();

        public void Reload() =>
            _inputService.Reload();

        private static IInputService RegisterInputService(BuildingsController buildingsController ,
            SkillController skillController)
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


