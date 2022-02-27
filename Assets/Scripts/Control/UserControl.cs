//#define TOUCH

using System;
using UnityEngine;



namespace Control
{
    public class UserControl : MonoBehaviour
    {
#if TOUCH
        private Touch _touch;
#endif
        private PlanetController _planetController;
        private SkillController _skillController;
        public bool isActive=false;
        public static UserControl Instance;
        public void Awake()
        {
            if (Instance==null)
            {
                Instance = this;
            }
        }

        public void Start()
        {
            _planetController = GetComponent<PlanetController>();
            _skillController = GetComponent<SkillController>();
        }

        public void Update()
        {
            if(!isActive)
                return;
#if TOUCH
            HandleTouch();
#else
            if(_skillController.IsSelectedSkill)
                _skillController.HandleClick();
            else 
                _planetController.HandleMouseClick(); 
#endif
       
            
        }
        
        
#if TOUCH
        private void HandleTouch()
        {
            if (Input.touchCount > 0)
            {
                //Debug.Log("touch screen\n");

                _touch = Input.GetTouch(0);
                if (_skillController.IsSelectedSkill)
                    _skillController.HandleTouch(_touch);
                else
                    _planetController.HandleTouch(_touch);
            }
        }
#endif
    }
}

    //handle ui selection
    //handle skill selection


