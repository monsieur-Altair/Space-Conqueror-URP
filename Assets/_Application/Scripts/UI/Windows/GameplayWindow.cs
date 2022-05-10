using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
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
        private List<Button> _skillButtons;

        [SerializeField] 
        private Transform _counterContainer;

        private ISkillController _skillController;
        private ICoroutineRunner _coroutineRunner;
        private Levels _levelManager;
        
        public Transform CounterContainer => _counterContainer;

        public override void GetDependencies()
        {
            base.GetDependencies();

            _skillController = AllServices.Instance.GetSingle<ISkillController>();
            _coroutineRunner = AllServices.Instance.GetSingle<ICoroutineRunner>();
            _levelManager = Levels.Instance;

            foreach (Button button in _skillButtons) 
                button.onClick.AddListener(() => { PressHandler(button); });

            _skillController.Acid.CanceledSkill += ()=> UnblockButton(_skillButtons[AcidIndex]);
            _skillController.Buff.CanceledSkill += ()=> UnblockButton(_skillButtons[BuffIndex]);
            _skillController.Call.CanceledSkill += ()=> UnblockButton(_skillButtons[CallIndex]);
            _skillController.Ice.CanceledSkill += ()=> UnblockButton(_skillButtons[IceIndex]);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            TeamManager.TeamCountUpdated += _teamBar.FillTeamCount;
            Buildings.Altar.ManaCountUpdated += _manaBar.FillManaCount;
        }

        protected override void UnSubscribeEvents()
        {
            base.UnSubscribeEvents();

            TeamManager.TeamCountUpdated -= _teamBar.FillTeamCount;
            Buildings.Altar.ManaCountUpdated -= _manaBar.FillManaCount;
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            ShowCertainUI();
        }

        private void ShowCertainUI()
        {
            int currentLevelNumber = Levels.Instance.CurrentLevelNumber; ///////////////////////////////

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
                    break;
                case 5:
                    break;
                default:
                    return;
            }
                                                                         
        }

        private void PressHandler(Button button)
        {
            if (_skillController.IsSkillNotSelected)
            {
                int index = _skillButtons.IndexOf(button);
                BlockButton(button);
                _coroutineRunner.StartCoroutine((SwitchWithWaiting(index)));
            }
            else
            {
                _skillController.ClearSelectedSkill();
                UnblockButton(button);
            }
        }

        private IEnumerator SwitchWithWaiting(int index)
        {
            //if not use waiting, touch "handle release" will be worked immediately
            yield return new WaitForSeconds(0.1f);//0.1s is finger lift time
            _skillController.SetSelectedSkill(index);
        }

        private static void BlockButton(Selectable button)
        {
            button.image.color = Color.red;
            button.enabled = false;
        }

        private static void UnblockButton(Selectable button)
        {
            button.enabled = true;
            button.image.color = Color.white;
        }
    }
}