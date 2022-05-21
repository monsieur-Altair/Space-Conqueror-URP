using System;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class Statistic
    {
        public int GainedMana;
        public int SpentMana;
        public int WinCount;
        public int UsedSpells;
        public int ConqueredBuildings;
        public int MissedBuildings;

        public Statistic()
        {
            GainedMana = 0;
            SpentMana = 0;
            WinCount = 0;
            UsedSpells = 0;
            ConqueredBuildings = 0;
            MissedBuildings = 0;
        }
    }
    
    
}