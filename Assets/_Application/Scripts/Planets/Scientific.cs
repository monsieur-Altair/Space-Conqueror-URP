using UnityEngine;

namespace Planets
{
    public class Scientific : Base
    {
        [SerializeField] private Scriptables.Scientific scientific;

        public static int MaxCountScientific { get; set; }
        private float ProduceCountScientific { get; set; }
        private float ProduceTimeScientific { get; set; }
        public static float ScientificCount { get; private set; }

        public static void DecreaseScientificCount(float value)
        {
            ScientificCount -= value;
        }
        
        protected override void LoadResources()
        {
            base.LoadResources();
            LoadScientificRes();
        }

        public static void DischargeScientificCount()
        {
            ScientificCount = 0;
        }
        
        private void LoadScientificRes()
        {
            MaxCountScientific = scientific.maxCount;
            ProduceCountScientific = scientific.produceCount;
            ProduceTimeScientific = scientific.produceTime;
        }

        protected override void IncreaseResources()
        {
            base.IncreaseResources();
            IncreaseScientificRes();
        }

        private void IncreaseScientificRes()
        {
            
            //CHANGE FOR EVERY TEAM
            if (Team == Team.Blue)
            {
                ScientificCount += ProduceCountScientific / ProduceTimeScientific * Time.deltaTime;
                if (ScientificCount > MaxCountScientific)
                    ScientificCount = MaxCountScientific;
            }

            if (Team == Team.Red)
            {
                AI.Core.ScientificCount+=ProduceCountScientific / ProduceTimeScientific * Time.deltaTime;
                if (AI.Core.ScientificCount > MaxCountScientific)
                    AI.Core.ScientificCount = MaxCountScientific;
            }
                
           
        }
    }
}