using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Control
{
    public enum SkillName
    {
        Buff=0,
        Acid,
        Ice,
        Call,
        None
    }
    
    public class SkillController : MonoBehaviour
    {
        [SerializeField] private List<Button> buttons;
        
        private const int Buff = 0, Acid = 1, Ice = 2, Call = 3;
        private Touch _touch;
        private SkillName _selectedSkillName;
        private Skills.Call _call;
        private Skills.Buff _buff;
        private Skills.Acid _acid;
        private Skills.Ice _ice;

        public static SkillController Instance;
        public Camera MainCamera { get; private set; }
        public static float MinDepth { get; private set; }
        public static float MaxDepth { get; private set; }
        public bool IsSelectedSkill { get; private set; }
        public event Action CanceledSelection; 
        
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void Start()
        {
            MainCamera=Camera.main;
            IsSelectedSkill = false;
            _selectedSkillName = SkillName.None;
            
            _call = buttons[Call].GetComponent<Skills.Call>();
            _buff = buttons[Buff].GetComponent<Skills.Buff>();
            _acid = buttons[Acid].GetComponent<Skills.Acid>();
            _ice = buttons[Ice].GetComponent<Skills.Ice>();
            
            MinDepth = MaxDepth = 0.0f;
            
            GetCameraDepths(out var min,out var max);
            MinDepth = min;
            MaxDepth = max;
        }

        public static void GetCameraDepths(out float min, out float max)
        {
            min = max = 0.0f;
            var camera = Camera.main;
            var plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            var ray = camera.ViewportPointToRay(new Vector3(0,0,0));
            if (plane.Raycast(ray, out var distance))
            {
                var botLeft = ray.GetPoint(distance);
                min = camera.WorldToViewportPoint(botLeft).z;
            }
            //Debug.Log("left="+MainCamera.WorldToViewportPoint(botLeft));
            
            ray = camera.ViewportPointToRay(new Vector3(1,1,0));
            if (plane.Raycast(ray, out var distance1))
            {
                var topRight = ray.GetPoint(distance1);
                max = camera.WorldToViewportPoint(topRight).z;
            }
            //Debug.Log("right="+MainCamera.WorldToViewportPoint(topRight));
            
            /*ray = camera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
            if (plane.Raycast(ray, out var distance2))
            {
                var center = ray.GetPoint(distance2);
                Debug.Log("center = "+center);
            }*/
        }
        
        public void HandleTouch(Touch touch)
        {
            _touch = touch;
           // Debug.Log(_touch.position);
           // Debug.Log("handle skill touch\n");
            
            switch (_touch.phase)
            {
                /*case TouchPhase.Began:
                {
                    HandleClick();
                    break;
                }*/
                case TouchPhase.Ended:
                {
                    HandleRelease();
                    break;
                }
            }
        }

        private void HandleRelease()
        {
            //if doesn't work after canceling button, will add yield return
            if (IsSelectedSkill)
            {
                var skill= ChooseSkill();
                skill.Execute(_touch.position);
                OnCanceledSelection();
            }
        }

        private Skills.ISkill ChooseSkill()
        {
            return _selectedSkillName switch
            {
                SkillName.Buff => _buff,
                SkillName.Acid => _acid,
                SkillName.Ice  => _ice,
                SkillName.Call => _call,
                SkillName.None => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void PressHandler(Button button)
        {
            if (!IsSelectedSkill)
            {
                BlockButton(button);
                StartCoroutine(nameof(SwitchWithWaiting));
            }
            else
            {
                OnCanceledSelection();
            }
        }

        private IEnumerator SwitchWithWaiting()
        {
            //if not use waiting, touch "handle release" will be worked immediately
            yield return new WaitForSeconds(0.1f);//0.1s is finger lift time
            IsSelectedSkill = true;
        }

        private void BlockButton(Button button)////////////////////////////////////////////
        {
            var index = buttons.IndexOf(button);
            _selectedSkillName = (SkillName)index;
            
            button.image.color=Color.red;
        }
       
        protected virtual void OnCanceledSelection()
        {
            CanceledSelection?.Invoke();
            _selectedSkillName = SkillName.None;
            IsSelectedSkill = false;
        }
    }
}