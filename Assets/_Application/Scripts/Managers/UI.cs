using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Misc;
using _Application.Scripts.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Application.Scripts.Managers
{
    public class UI
    {
        private readonly Canvas _canvas;
        private readonly ObjectPool _pool;
        private readonly Warehouse _warehouse;
        private readonly Control.SkillController _skillController;

        private readonly List<Color> _counterBackground;
        private readonly List<Color> _counterForeground;

        private List<Planets.Base> _allPlanets = new List<Planets.Base>();
        private readonly List<GameObject> _countersList = new List<GameObject>();
        private readonly Dictionary<int, Image> _backgrounds = new Dictionary<int, Image>();
        private readonly Dictionary<int, TextMeshProUGUI> _foregrounds = new Dictionary<int, TextMeshProUGUI>();

        private GameObject _toUpgradeMenuButton;
        private GameObject _nextLevelButton;
        private GameObject _retryLevelButton;
        private GameObject _scientificBar;
        private GameObject _teamBar;

        private List<Button> _skillButtons;

        private TextMeshProUGUI _moneyText;

        private readonly Vector3 _offset = new Vector3(0, -35, 0);
        private readonly Vector3 _baseCounterScale = new Vector3(1, 1, 1);

        private bool _isSkillButtonsActive;
        private GameObject _upgradeMenu;

        private const int BuffIndex = 0;
        private const int AcidIndex = 1;
        private const int IceIndex = 2;
        private const int CallIndex = 3;

        public UI(Canvas canvas, ObjectPool pool, Warehouse warehouse, Control.SkillController skillController)
        {
            _canvas = canvas;
            _pool = pool;
            _warehouse = warehouse;
            _skillController = skillController;
            _counterBackground = _warehouse.counterBackground;
            _counterForeground = _warehouse.counterForeground;
        }

        public void PrepareLevel(List<Planets.Base> allPlanets)
        {
            ClearList();
            _allPlanets = new List<Planets.Base>(allPlanets);
            FillLists();
        }

        public void EnableSkillUI() => 
            _isSkillButtonsActive = true;

        public void DisableSkillUI() => 
            _isSkillButtonsActive = false;

        public void SetButtons(List<Button> skillButtons, GameObject retryButton, GameObject nextLevelButton, GameObject toUpgradeMenuButton)
        {
            _retryLevelButton = retryButton;
            _nextLevelButton = nextLevelButton;
            _toUpgradeMenuButton = toUpgradeMenuButton;
            
            _toUpgradeMenuButton.SetActive(false);
            _retryLevelButton.SetActive(false);
            _nextLevelButton.SetActive(false);

            _skillButtons = skillButtons;
        }

        public void SetUIBehaviours(TeamManager teamManager ,UnityAction retryLevelBehaviour, 
            UnityAction loadNextLevelBehaviour, UnityAction toUpgradeMenuBehaviour, UnityAction backToGame)
        {
            _retryLevelButton.GetComponent<Button>().onClick.AddListener(retryLevelBehaviour);
            _nextLevelButton.GetComponent<Button>().onClick.AddListener(loadNextLevelBehaviour);
            _toUpgradeMenuButton.GetComponent<Button>().onClick.AddListener(toUpgradeMenuBehaviour);

            _upgradeMenu.GetComponent<UpgradeMenuController>().backButton.onClick.AddListener(backToGame);

            
            AdjustSkillButtons();
            
            teamManager.TeamCountUpdated += _teamBar.GetComponent<TeamProgressBar>().FillTeamCount;
            Planets.Scientific.ScientificCountUpdated += _scientificBar.GetComponent<ScientificBar>().FillScientificCount;
        }

        public static void RemoveBehaviours(TeamManager teamManager)
        {
            Planets.Base.Conquered -= teamManager.UpdateObjectsCount;
            //_teamManager.TeamCountUpdated -= _teamBar.GetComponent<TeamProgressBar>().FillTeamCount;
            //null refs?
            //Planets.Scientific.ScientificCountUpdated -= _scientificBar.GetComponent<ScientificBar>().FillScientificCount;
        }

        public void SetBars(GameObject scientificBar, GameObject teamBar)
        {
            _scientificBar = scientificBar;
            _teamBar = teamBar;
        }

        public void SetText(TextMeshProUGUI moneyText) => 
            _moneyText = moneyText;

        public void SetUpgradeMenu(GameObject upgradeMenu)
        {
            _upgradeMenu = upgradeMenu;
            _upgradeMenu.SetActive(false);
        }

        public void HideSkillsButtons()
        {
            foreach (Button skillButton in _skillButtons) 
                skillButton.gameObject.SetActive(false);
        }

        public void ShowSkillsButtons()
        {
            foreach (Button skillButton in _skillButtons) 
                skillButton.gameObject.SetActive(true);
        }

        public void HideUpgradeMenu()
        {
            _upgradeMenu.SetActive(false);
        }

        public void ShowUpgradeMenu()
        {
            _upgradeMenu.SetActive(true);
        }

        public void ShowGameplayUI()
        {
            _scientificBar.SetActive(true);
            _teamBar.SetActive(true);
        }

        public void HideGameOverUI()
        {
            _toUpgradeMenuButton.SetActive(false);
            _nextLevelButton.SetActive(false);
            _retryLevelButton.SetActive(false);
            _moneyText.gameObject.SetActive(false);
        }

        public void ShowGameOverUI(bool isWin)
        {
            if (isWin)
                _nextLevelButton.SetActive(true);
            else
                _retryLevelButton.SetActive(true);
            
            _toUpgradeMenuButton.SetActive(true);
            _moneyText.gameObject.SetActive(true);
        }

        public void HideGameplayUI()
        {
            _scientificBar.SetActive(false);
            _teamBar.SetActive(false);
        }

        public void UpdateMoneyText(int money) => 
            _moneyText.text = $"money: {money}";

        private void AdjustSkillButtons()
        {
            foreach (Button button in _skillButtons) 
                button.onClick.AddListener(() => { PressHandler(button); });

            _skillController.Acid.CanceledSkill += ()=> UnblockButton(_skillButtons[AcidIndex]);
            _skillController.Buff.CanceledSkill += ()=> UnblockButton(_skillButtons[BuffIndex]);
            _skillController.Call.CanceledSkill += ()=> UnblockButton(_skillButtons[CallIndex]);
            _skillController.Ice.CanceledSkill += ()=> UnblockButton(_skillButtons[IceIndex]);
        }

        private void PressHandler(Button button)
        {
            if(!_isSkillButtonsActive) 
                return;
            
            if (_skillController.IsSkillNotSelected)
            {
                int index = _skillButtons.IndexOf(button);
                BlockButton(button);
                GlobalObject.Instance.StartCoroutine((SwitchWithWaiting(index)));
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

        private void ClearList()
        {
            foreach (GameObject counter in _countersList) 
                counter.gameObject.SetActive(false);
            
            foreach (Planets.Base planet in _allPlanets)
            {
                planet.CountChanged -= SetCounter;
                planet.TeamChanged -= SetCounterColor;
            }
            _allPlanets.Clear();
            _foregrounds.Clear();
            _backgrounds.Clear();
        }

        private void FillLists()
        {
            foreach (Planets.Base planet in _allPlanets)
            {
                planet.CountChanged += SetCounter;
                planet.TeamChanged += SetCounterColor;

                GameObject counter = SpawnCounterTo(planet);
                DecomposeCounter(counter, planet);
                
                SetCounterColor(planet);
                SetCounter(planet, (int) planet.Count);
            }
        }

        private void DecomposeCounter(GameObject counter, Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            _foregrounds.Add(index, counter.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>());
            _backgrounds.Add(index, counter.GetComponentInChildren<Image>());
        }

        private GameObject SpawnCounterTo(Planets.Base planet)
        {
            Vector3 counterPos = GetCounterPos(planet);
            GameObject counter = _pool.GetObject(PoolObjectType.Counter, counterPos, Quaternion.identity);
            counter.transform.SetParent(_canvas.transform);
            counter.transform.localScale = _baseCounterScale;
            _countersList.Add(counter);
            return counter;
        }

        private Vector3 GetCounterPos(Planets.Base planet)
        {
            Vector3 pos = planet.transform.position;
            Vector3 screenPos = CameraResolution.GetScreenPos(pos);
            return screenPos + _offset;
        }

        private void SetCounterColor(Planets.Base planet)
        {
            int team = (int) planet.Team;
            int index = planet.ID.GetHashCode();
            _foregrounds[index].color = _counterForeground[team];
            _backgrounds[index].color = _counterBackground[team];
        }

        private void SetCounter(Planets.Base planet, int value)
        {
            int index = planet.ID.GetHashCode();
            _foregrounds[index].text = value.ToString();
        }

        private static void BlockButton(Button button)
        {
            button.image.color = Color.red;
            button.enabled = false;
        }

        private static void UnblockButton(Button button)
        {
            button.enabled = true;
            button.image.color = Color.white;
        }
    }
}