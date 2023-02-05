using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class GameplayWindow : Window
    {
        private const int BuffIndex = 0;
        private const int AcidIndex = 1;
        private const int IceIndex = 2;
        private const int CallIndex = 3;

        [SerializeField] 
        private ManaBar _manaBar;

        [SerializeField] 
        private TeamProgressBar _teamBar;

        [SerializeField] 
        private List<SkillButton> _skillButtons;

        [SerializeField] 
        private Transform _counterContainer;

        private SkillController _skillController;
        private CoroutineRunner _coroutineRunner;
        private LevelManager _levelManager;
        private ScriptableService _scriptableService;
        private Mana _manaInfo;

        public Transform CounterContainer => _counterContainer;

        public override void GetDependencies()
        {
            base.GetDependencies();

            _skillController = AllServices.Get<SkillController>();
            _coroutineRunner = AllServices.Get<CoroutineRunner>();
            _levelManager = AllServices.Get<LevelManager>();
            _scriptableService = AllServices.Get<ScriptableService>();
            _manaInfo = _scriptableService.GetManaInfo(Team.Blue);

            _skillController.Acid.CanceledSkill += ()=> UnlockButton(_skillButtons[AcidIndex]);
            _skillController.Buff.CanceledSkill += ()=> UnlockButton(_skillButtons[BuffIndex]);
            _skillController.Call.CanceledSkill += ()=> UnlockButton(_skillButtons[CallIndex]);
            _skillController.Ice.CanceledSkill += ()=> UnlockButton(_skillButtons[IceIndex]);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            TeamManager.TeamCountUpdated += _teamBar.FillTeamCount;
            Altar.ManaCountUpdated += _manaBar.FillManaCount;
            
            foreach (SkillButton skillButton in _skillButtons) 
                skillButton.Clicked += PressHandler;
        }

        protected override void UnSubscribeEvents()
        {
            base.UnSubscribeEvents();

            TeamManager.TeamCountUpdated -= _teamBar.FillTeamCount;
            Altar.ManaCountUpdated -= _manaBar.FillManaCount;
            
            foreach (SkillButton skillButton in _skillButtons) 
                skillButton.Clicked -= PressHandler;
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            ShowCertainUI();

            foreach (SkillButton skillButton in _skillButtons) 
                skillButton.OnOpened();
            
            _manaBar.FillManaCount(0, _manaInfo.maxCount);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            foreach (SkillButton skillButton in _skillButtons) 
                skillButton.OnClosed();
        }

        private void ShowCertainUI()
        {
            int currentLevelNumber = _levelManager.CurrentLevelNumber;
            currentLevelNumber = 5;
            switch (currentLevelNumber)
            {
                case 0:
                case 1:
                    _manaBar.gameObject.SetActive(false);
                    _skillButtons[AcidIndex].gameObject.SetActive(false);
                    _skillButtons[BuffIndex].gameObject.SetActive(false);
                    _skillButtons[CallIndex].gameObject.SetActive(false);
                    _skillButtons[IceIndex ].gameObject.SetActive(false);
                    break;
                case 2:
                    _manaBar.gameObject.SetActive(true);
                    _skillButtons[AcidIndex].gameObject.SetActive(false);
                    _skillButtons[BuffIndex].gameObject.SetActive(true);
                    _skillButtons[CallIndex].gameObject.SetActive(false);
                    _skillButtons[IceIndex ].gameObject.SetActive(false);
                    break;
                case 3:
                    _manaBar.gameObject.SetActive(true);
                    _skillButtons[AcidIndex].gameObject.SetActive(true);
                    _skillButtons[BuffIndex].gameObject.SetActive(true);
                    _skillButtons[CallIndex].gameObject.SetActive(false);
                    _skillButtons[IceIndex ].gameObject.SetActive(false);
                    break;
                case 4:
                    _manaBar.gameObject.SetActive(true);
                    _skillButtons[AcidIndex].gameObject.SetActive(true);
                    _skillButtons[BuffIndex].gameObject.SetActive(true);
                    _skillButtons[CallIndex].gameObject.SetActive(false);
                    _skillButtons[IceIndex ].gameObject.SetActive(true);
                    break;
                case 5:
                    _manaBar.gameObject.SetActive(true);
                    _skillButtons[AcidIndex].gameObject.SetActive(true);
                    _skillButtons[BuffIndex].gameObject.SetActive(true);
                    _skillButtons[CallIndex].gameObject.SetActive(true);
                    _skillButtons[IceIndex ].gameObject.SetActive(true);
                    break;
                default:
                    return;
            }
                                                                         
        }

        private void PressHandler(SkillButton skillButton)
        {
            if (_skillController.IsSkillNotSelected)
            {
                int index = _skillButtons.IndexOf(skillButton);
                BlockButton(skillButton);
                _coroutineRunner.StartCoroutine((SwitchWithWaiting(index)));
            }
            else
            {
                _skillController.ClearSelectedSkill();
                UnlockButton(skillButton);
            }
        }

        private IEnumerator SwitchWithWaiting(int index)
        {
            //if not use waiting, touch "handle release" will be worked immediately
            yield return new WaitForSeconds(0.1f);//0.1s is finger lift time
            _skillController.SetSelectedSkill(index);
        }

        private static void BlockButton(SkillButton button)
        {
            button.Mask.color = new Color(0.58f,0,0.06f,0.47f);
            button.Button.interactable = false;
        }

        private static void UnlockButton(SkillButton button)
        {
            button.Mask.color = new Color(0.58f,0,0.06f,0.0f);
            button.Button.interactable = true;
        }
    }
}