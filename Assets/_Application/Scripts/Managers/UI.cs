using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Application.Scripts.Managers
{
    public class UI
    {
        //private readonly Control.SkillController _skillController;
        
        private GameObject _toUpgradeMenuButton;
        private GameObject _nextLevelButton;
        private GameObject _retryLevelButton;
        //private GameObject _manaBar;
       // private GameObject _teamBar;

        //private List<Button> _skillButtons;

        private TextMeshProUGUI _moneyText;

       

       // private bool _isSkillButtonsActive;
        private GameObject _upgradeMenu;

        // private const int BuffIndex = 0;
        // private const int AcidIndex = 1;
        // private const int IceIndex = 2;
        // private const int CallIndex = 3;

        // public UI(Control.SkillController skillController) => 
        //     _skillController = skillController;
        
        // public void EnableSkillUI() => 
        //     _isSkillButtonsActive = true;
        //
        // public void DisableSkillUI() => 
        //     _isSkillButtonsActive = false;

        public void SetButtons(List<Button> skillButtons, GameObject retryButton, GameObject nextLevelButton, GameObject toUpgradeMenuButton)
        {
            _retryLevelButton = retryButton;
            _nextLevelButton = nextLevelButton;
            _toUpgradeMenuButton = toUpgradeMenuButton;
            
            _toUpgradeMenuButton.SetActive(false);
            _retryLevelButton.SetActive(false);
            _nextLevelButton.SetActive(false);

            //_skillButtons = skillButtons;
        }

        public void SetUIBehaviours(TeamManager teamManager ,UnityAction retryLevelBehaviour, 
            UnityAction loadNextLevelBehaviour, UnityAction toUpgradeMenuBehaviour, UnityAction backToGame)
        {
            _retryLevelButton.GetComponent<Button>().onClick.AddListener(retryLevelBehaviour);
            _nextLevelButton.GetComponent<Button>().onClick.AddListener(loadNextLevelBehaviour);
            _toUpgradeMenuButton.GetComponent<Button>().onClick.AddListener(toUpgradeMenuBehaviour);

            //_upgradeMenu.GetComponent<UpgradeWindow>().backButton.onClick.AddListener(backToGame);

            
            //AdjustSkillButtons();
            
            //TeamManager.TeamCountUpdated += _teamBar.GetComponent<TeamProgressBar>().FillTeamCount;
            //Buildings.Altar.ManaCountUpdated += _manaBar.GetComponent<ManaBar>().FillManaCount;
        }

        // public static void RemoveBehaviours(TeamManager teamManager)
        // {
        //     Buildings.Base.Conquered -= teamManager.UpdateObjectsCount;
        //     //_teamManager.TeamCountUpdated -= _teamBar.GetComponent<TeamProgressBar>().FillTeamCount;
        //     //null refs?
        //     //Planets.Mana.ScientificCountUpdated -= _manaBar.GetComponent<ManaBar>().FillScientificCount;
        // }

        // public void SetBars(GameObject scientificBar, GameObject teamBar)
        // {
        //     _manaBar = scientificBar;
        //     _teamBar = teamBar;
        // }

        public void SetText(TextMeshProUGUI moneyText) => 
            _moneyText = moneyText;

        // public void SetUpgradeMenu(GameObject upgradeMenu)
        // {
        //     _upgradeMenu = upgradeMenu;
        //     _upgradeMenu.SetActive(false);
        // }

        // public void HideSkillsButtons()
        // {
        //     foreach (Button skillButton in _skillButtons) 
        //         skillButton.gameObject.SetActive(false);
        // }
        //
        // public void ShowSkillsButtons()
        // {
        //     foreach (Button skillButton in _skillButtons) 
        //         skillButton.gameObject.SetActive(true);
        // }

        // public void HideUpgradeMenu() => 
        //     _upgradeMenu.SetActive(false);

        // public void ShowUpgradeMenu() => 
        //     _upgradeMenu.SetActive(true);

        // public void ShowGameplayUI()
        // {
	       //  _manaBar.SetActive(true);
        //     _teamBar.SetActive(true);
        // }

        // public void HideGameOverUI()
        // {
        //     _toUpgradeMenuButton.SetActive(false);
        //     _nextLevelButton.SetActive(false);
        //     _retryLevelButton.SetActive(false);
        //     _moneyText.gameObject.SetActive(false);
        // }

        // public void ShowGameOverUI(bool isWin)
        // {
        //     if (isWin)
        //         _nextLevelButton.SetActive(true);
        //     else
        //         _retryLevelButton.SetActive(true);
        //     
        //     _toUpgradeMenuButton.SetActive(true);
        //     _moneyText.gameObject.SetActive(true);
        // }
        
        
        // public void HideGameplayUI()
        // {
 	      //   _manaBar.SetActive(false);
        //     _teamBar.SetActive(false);
        // }

        public void UpdateMoneyText(int money) => 
            _moneyText.text = $"money: {money}";

        // private void AdjustSkillButtons()
        // {
        //     foreach (Button button in _skillButtons) 
        //         button.onClick.AddListener(() => { PressHandler(button); });
        //
        //     _skillController.Acid.CanceledSkill += ()=> UnblockButton(_skillButtons[AcidIndex]);
        //     _skillController.Buff.CanceledSkill += ()=> UnblockButton(_skillButtons[BuffIndex]);
        //     _skillController.Call.CanceledSkill += ()=> UnblockButton(_skillButtons[CallIndex]);
        //     _skillController.Ice.CanceledSkill += ()=> UnblockButton(_skillButtons[IceIndex]);
        // }

        // private void PressHandler(Button button)
        // {
        //     // if(!_isSkillButtonsActive) 
        //     //     return;
        //     
        //     if (_skillController.IsSkillNotSelected)
        //     {
        //         int index = _skillButtons.IndexOf(button);
        //         BlockButton(button);
        //         GlobalObject.Instance.StartCoroutine((SwitchWithWaiting(index)));
        //     }
        //     else
        //     {
        //         _skillController.ClearSelectedSkill();
        //         UnblockButton(button);
        //     }
        // }

        // private IEnumerator SwitchWithWaiting(int index)
        // {
        //     //if not use waiting, touch "handle release" will be worked immediately
        //     yield return new WaitForSeconds(0.1f);//0.1s is finger lift time
        //     _skillController.SetSelectedSkill(index);
        // }
        
        // private static void BlockButton(Button button)
        // {
        //     button.image.color = Color.red;
        //     button.enabled = false;
        // }
        //
        // private static void UnblockButton(Button button)
        // {
        //     button.enabled = true;
        //     button.image.color = Color.white;
        // }
    }
}