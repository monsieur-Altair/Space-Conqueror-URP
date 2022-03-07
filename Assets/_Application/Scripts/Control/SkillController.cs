using System;
using System.Collections;
using System.Collections.Generic;
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
    [DefaultExecutionOrder(450)]
    public class SkillController : MonoBehaviour
    {
        [SerializeField] private List<Button> buttons;

        private static SkillController _instance;

        private const int Buff = 0;
        private const int Acid = 1;
        private const int Ice = 2;
        private const int Call = 3;

        public SkillName SelectedSkillName { get; private set; }
        private Skills.Call _call;
        private Skills.Buff _buff;
        private Skills.Acid _acid;
        private Skills.Ice _ice;
        
        private bool _isActive;


        public void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        public void Start()
        {
            SelectedSkillName = SkillName.None;
            
            _call = buttons[Call].GetComponent<Skills.Call>();
            _call.SetTeamConstraint(Planets.Team.Blue);
            _call.SetDecreasingFunction(Planets.Scientific.DecreaseScientificCount);
            _call.CanceledSkill += ()=> UnblockButton(buttons[Call]);
            
            _buff = buttons[Buff].GetComponent<Skills.Buff>();
            _buff.SetTeamConstraint(Planets.Team.Blue);
            _buff.SetDecreasingFunction(Planets.Scientific.DecreaseScientificCount);
            _buff.CanceledSkill += ()=> UnblockButton(buttons[Buff]);
            
            _acid = buttons[Acid].GetComponent<Skills.Acid>();
            _acid.SetTeamConstraint(Planets.Team.Blue);
            _acid.SetDecreasingFunction(Planets.Scientific.DecreaseScientificCount);
            _acid.CanceledSkill += ()=> UnblockButton(buttons[Acid]);
            
            _ice = buttons[Ice].GetComponent<Skills.Ice>();
            _ice.CanceledSkill += ()=> UnblockButton(buttons[Ice]);
            
            foreach (Button button in buttons) 
                button.onClick.AddListener(() => { PressHandler(button); });

        }
        
        public void ApplySkill(Vector3 position)
        {
            if (SelectedSkillName!=SkillName.None)
            {
                Skills.ISkill skill= ChooseSkill();
                skill.ExecuteForPlayer(position);
                SelectedSkillName = SkillName.None;
            }
        }

        public void Disable()
        {
            _isActive = false;
        }

        public void Enable()
        {
            _isActive = true;
        }

        private void PressHandler(Button button)
        {
            if(!_isActive) 
                return;
            
            if (SelectedSkillName==SkillName.None)
            {
                int index = buttons.IndexOf(button);
                BlockButton(button);
                StartCoroutine((SwitchWithWaiting(index)));
            }
            else
            {
                SelectedSkillName = SkillName.None;
                UnblockButton(button);
            }
        }

        private Skills.ISkill ChooseSkill()
        {
            return SelectedSkillName switch
            {
                SkillName.Buff => _buff,
                SkillName.Acid => _acid,
                SkillName.Ice  => _ice,
                SkillName.Call => _call,
                SkillName.None => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private IEnumerator SwitchWithWaiting(int index)
        {
            //if not use waiting, touch "handle release" will be worked immediately
            yield return new WaitForSeconds(0.1f);//0.1s is finger lift time
            SelectedSkillName = (SkillName)index;
        }

        private static void BlockButton(Button button)
            => button.image.color=Color.red;

        private static void UnblockButton(Button button) => 
            button.image.color = Color.white;
    }
}