﻿using UnityEngine;

namespace _Application.Scripts.Control
{
    public class UserControl : MonoBehaviour
    {
        private bool _isActive;

        private IInputService _inputService;

        public void Init(BuildingsController buildingsController , ISkillController skillController)
        {
            //_inputService = Game.InputService;
            //_inputService.Init(new BuildingsController(MainCamera.main), GetComponent<SkillController>());
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

        public void Enable() => 
            _isActive = true;

        public void Refresh() => 
            _inputService.Refresh();

        public void Reload() =>
            _inputService.Reload();

        private static IInputService RegisterInputService(BuildingsController buildingsController ,
            ISkillController skillController)
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


