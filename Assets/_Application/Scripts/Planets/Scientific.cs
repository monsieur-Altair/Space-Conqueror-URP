using UnityEngine;

namespace _Application.Scripts.Planets
{
    public class Scientific : Base
    {
        [SerializeField] private Scriptables.Scientific scientific;

        private static int _maxCountScientific;
        private float _produceCountScientific;
        private float _produceTimeScientific;
        
        public static float ScientificCount { get; private set; }

        public static void DecreaseScientificCount(float value) => 
            ScientificCount -= value;

        public static void DischargeScientificCount() => 
            ScientificCount = 0;

        protected override void LoadResources()
        {
            base.LoadResources();
            LoadScientificRes();
        }

        protected override void IncreaseResources()
        {
            base.IncreaseResources();
            IncreaseScientificRes();
        }

        private void LoadScientificRes()
        {
            _maxCountScientific = scientific.maxCount;
            _produceCountScientific = scientific.produceCount;
            _produceTimeScientific = scientific.produceTime;
        }

        private void IncreaseScientificRes()
        {
            //CHANGE FOR EVERY TEAM
            if (Team == Team.Blue) 
                IncreaseForPlayer();

            if (Team == Team.Red) 
                IncreaseForAI();

        }

        private void IncreaseForAI()
        {
            AI.Core.ScientificCount += _produceCountScientific / _produceTimeScientific * Time.deltaTime;
            if (AI.Core.ScientificCount > _maxCountScientific)
                AI.Core.ScientificCount = _maxCountScientific;
        }

        private void IncreaseForPlayer()
        {
            ScientificCount += _produceCountScientific / _produceTimeScientific * Time.deltaTime;
            if (ScientificCount > _maxCountScientific)
                ScientificCount = _maxCountScientific;
        }
    }
}