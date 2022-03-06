using Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    [DefaultExecutionOrder(1000)]
    public abstract class Base : MonoBehaviour, ISkill
    {
        [SerializeField] protected Skill resource;
        protected Vector3 SelectedScreenPos { get; private set; }
        protected Control.SkillController SkillController;
        protected Camera MainCamera;
        protected Managers.ObjectPool ObjectPool;
        protected Planets.Base SelectedPlanet;
        protected abstract void ApplySkill();
        protected abstract void CancelSkill();
        protected bool IsOnCooldown = false;
        protected bool IsForAI = false;
        protected Planets.Team TeamConstraint; 
        protected delegate void UniqueActionToPlanet();
        public delegate void DecreasingCounter(float count);

        private DecreasingCounter _decreaseCounter;
        public float Cooldown { get; private set; }
        public int Cost { get; private set; }

        private Button _button; 
        public void Start()
        {
            SkillController = Control.SkillController.Instance;
            if (SkillController == null)
                throw new MyException("can't get skill controller");
            MainCamera=SkillController.MainCamera;
            if(MainCamera==null)
                throw new MyException("can't get main camera");
            ObjectPool = Managers.ObjectPool.Instance;
            if (ObjectPool == null)
                throw new MyException("can't get object pool");
            _button = GetComponent<Button>();
            if(_button!=null)
                _button.onClick.AddListener(() => { SkillController.PressHandler(_button);});
            
            SkillController.CanceledSelection += UnblockButton;
            
            LoadResources();
        }

        protected virtual void LoadResources()
        {
            Cooldown = resource.cooldown;
            Cost = resource.cost;
        }

        public void SetDecreasingFunction(DecreasingCounter function)
        {
            _decreaseCounter = function;
        }
        
        public void ExecuteForPlayer(Vector3 pos)
        {
            SelectedScreenPos = pos;
            if (Planets.Scientific.ScientificCount>Cost && !IsOnCooldown)
            {
                ApplySkill();
            }
            else
            {
                UnblockButton();
            }
        }

        public void SetTeamConstraint(Planets.Team team)
        {
            IsForAI = (team == Planets.Team.Red);
            TeamConstraint = team;
        }

        public void ExecuteForAI(Planets.Base planet)
        {
            SelectedPlanet = planet;
            if (AI.Core.ScientificCount > Cost && !IsOnCooldown)
            {
                ApplySkill();
            }
        }

        protected Planets.Base RaycastForPlanet()
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            var ray = MainCamera.ScreenPointToRay(SelectedScreenPos);
            return Physics.Raycast(ray, out var hit,Mathf.Infinity, layerMask) ? hit.collider.GetComponent<Planets.Base>() : null;
        }
        
        protected void UnblockButton()
        {
            if(!IsOnCooldown && _button!=null)
                _button.image.color=Color.white;
        }

        protected void ApplySkillToPlanet(UniqueActionToPlanet action)
        {
            if (SelectedPlanet != null)
            {
                IsOnCooldown = true;
                _decreaseCounter(Cost);
                action();
                Invoke(nameof(CancelSkill), Cooldown);
            }
            else
            {
                UnblockButton();
            }
        }
        
    }
}