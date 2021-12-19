using UnityEditor;
using UnityEngine;

namespace Control
{
    public class UserControl : MonoBehaviour
    {
        private Touch _touch;
        private PlanetController _planetController;
        private SkillController _skillController;
    
        public void Start()
        {
            _planetController = GetComponent<PlanetController>();
            _skillController = GetComponent<SkillController>();
        }

        public void Update()
        {
            if (Input.touchCount > 0)
            {

                //Debug.Log("touch screen\n");
                
                _touch = Input.GetTouch(0);
                if(_skillController.IsSelectedSkill)
                    _skillController.HandleTouch(_touch);
                else
                    _planetController.HandleTouch(_touch); 
            }
        }
    }
}

    //handle ui selection
    //handle skill selection


