using System;
using _Application.Scripts.Scriptables;
using UnityEngine;

namespace _Application.Scripts.Buildings
{
    public class Altar : BaseBuilding
    {
        public static event Action<float, int> ManaCountUpdated = delegate {  }; 
        
        private static int _maxCountMana;
        private float _produceCountMana;
        private float _produceTimeMana;
        
        public static float ManaCount { get; private set; }
        public static float SavedManaCount { get; private set; }

        public static void DecreaseManaCount(float value)
        {
            ManaCount -= value;
            ManaCountUpdated(ManaCount, _maxCountMana);
        }

        public static void DischargeManaCount()
        {
            ManaCount = 0;
            ManaCountUpdated(ManaCount, _maxCountMana);
        }

        public static void DischargeSavedManaCount() => 
            SavedManaCount = 0;

        protected override void LoadResources(Building infoAboutBuilding, Unit infoAboutUnit)
        {
            base.LoadResources(infoAboutBuilding, infoAboutUnit);
            Team availableTeam = (Team == Team.White) ? Team.Red : Team;
            LoadAltarRes(ScriptableService.GetManaInfo(availableTeam));
        }

        protected override void IncreaseResources()
        {
            base.IncreaseResources();
            IncreaseManaRes();
        }

        private void LoadAltarRes(Mana mana)
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
            float producedManaCount = _produceCountMana / _produceTimeMana * Time.deltaTime;
            ManaCount += producedManaCount;
            if (ManaCount > _maxCountMana)
            {
                ManaCount = _maxCountMana;
                producedManaCount = 0;
            }
            SavedManaCount += producedManaCount;
            ManaCountUpdated(ManaCount, _maxCountMana);
        }
    }
}