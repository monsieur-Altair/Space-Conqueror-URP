using System;
using UnityEngine;

namespace _Application.Scripts.Buildings
{
    public class Altar : Base
    {
        [SerializeField] private Scriptables.Mana mana;

        public static event Action<float, int> ManaCountUpdated; 
        
        private static int _maxCountMana;
        private float _produceCountMana;
        private float _produceTimeMana;
        
        public static float ManaCount { get; private set; }

        public static void DecreaseManaCount(float value)
        {
            ManaCount -= value;
            ManaCountUpdated?.Invoke(ManaCount, _maxCountMana);
        }

        public static void DischargeManaCount()
        {
            ManaCount = 0;
            ManaCountUpdated?.Invoke(ManaCount, _maxCountMana);
        }

        protected override void LoadResources()
        {
            base.LoadResources();
            LoadAltarRes();
        }

        protected override void IncreaseResources()
        {
            base.IncreaseResources();
            IncreaseManaRes();
        }

        private void LoadAltarRes()
        {
            _maxCountMana = mana.maxCount;
            _produceCountMana = mana.produceCount;
            _produceTimeMana = mana.produceTime;
        }

        private void IncreaseManaRes()
        {
            //CHANGE FOR EVERY TEAM
            if (Team == Team.Blue) 
                IncreaseForPlayer();

            if (Team == Team.Red) 
                IncreaseForAI();
        }

        private void IncreaseForAI()
        {
            AI.Core.ManaCount += _produceCountMana / _produceTimeMana * Time.deltaTime;
            if (AI.Core.ManaCount > _maxCountMana)
                AI.Core.ManaCount = _maxCountMana;
        }

        private void IncreaseForPlayer()
        {
            ManaCount += _produceCountMana / _produceTimeMana * Time.deltaTime;
            if (ManaCount > _maxCountMana)
                ManaCount = _maxCountMana;
            ManaCountUpdated?.Invoke(ManaCount, _maxCountMana);
        }
    }
}