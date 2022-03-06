using System;
using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Misc;
using Control;
using Skills;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace _Application.Scripts.Control
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

        
        public static SkillController Instance;
        public Camera MainCamera { get; private set; }
        public static float MinDepth { get; private set; }
        public static float MaxDepth { get; private set; }
        public bool IsSelectedSkill { get; private set; }
        public event Action CanceledSelection;
        
        
        private const int Buff = 0;
        private const int Acid = 1;
        private const int Ice = 2;
        private const int Call = 3;
        
        private SkillName _selectedSkillName;
        private Skills.Call _call;
        private Skills.Buff _buff;
        private Skills.Acid _acid;
        private Skills.Ice _ice;

        
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
            _call.SetTeamConstraint(Planets.Team.Blue);
            _call.SetDecreasingFunction(Planets.Scientific.DecreaseScientificCount);
            
            _buff = buttons[Buff].GetComponent<Skills.Buff>();
            _buff.SetTeamConstraint(Planets.Team.Blue);
            _buff.SetDecreasingFunction(Planets.Scientific.DecreaseScientificCount);
            
            _acid = buttons[Acid].GetComponent<Skills.Acid>();
            _acid.SetTeamConstraint(Planets.Team.Blue);
            _acid.SetDecreasingFunction(Planets.Scientific.DecreaseScientificCount);

            
            _ice = buttons[Ice].GetComponent<Skills.Ice>();
            
            MinDepth = MaxDepth = 0.0f;
            
            CameraResolution.GetCameraDepths(out float min,out float max);
            MinDepth = min;
            MaxDepth = max;
        }
        
        public void ApplySkill(Vector3 position)
        {
            if (IsSelectedSkill)
            {
                ISkill skill= ChooseSkill();
                skill.ExecuteForPlayer(position);
                OnCanceledSelection();
            }
        }

        public void PressHandler(Button button)
        {
            if(!UserControl.Instance.isActive)
                return;
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

        private ISkill ChooseSkill()
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

        private void OnCanceledSelection()
        {
            CanceledSelection?.Invoke();
            _selectedSkillName = SkillName.None;
            IsSelectedSkill = false;
        }
    }
}