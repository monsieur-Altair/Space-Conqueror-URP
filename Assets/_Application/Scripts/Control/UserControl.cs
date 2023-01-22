using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public class UserControl : MonoBehaviourService
    {
        private bool _isActive;
        public IInputService InputService { get; private set; }

        public override void Init()
        {
            base.Init();
            
            BuildingsController buildingsController = new BuildingsController(
                AllServices.Get<GlobalCamera>().MainCamera, 
                AllServices.Get<CoreConfig>(), 
                AllServices.Get<GlobalPool>());
            
            InputService = RegisterInputService(buildingsController, AllServices.Get<SkillController>());
        }

        public void Update()
        {
            if(!_isActive)
                return;
            
            InputService.HandleInput();
        }

        public void Disable() => 
            _isActive = false;

        public void Enable() => 
            _isActive = true;

        public void Refresh() => 
            InputService.Refresh();

        public void Reload() =>
            InputService.Reload();

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


