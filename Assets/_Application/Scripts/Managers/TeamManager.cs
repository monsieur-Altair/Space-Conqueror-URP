using System;
using System.Collections.Generic;

namespace _Application.Scripts.Managers
{
    public class TeamManager
    {
        public event Action<List<int>> TeamCountUpdated;
        public event Action<bool> GameEnded;

        private readonly int _teamNumber = Enum.GetNames(typeof(Planets.Team)).Length;
        private readonly List<int> _teamCount;

        public TeamManager()
        {
            _teamCount = new List<int>(_teamNumber);
            for (int i = 0; i < _teamNumber;i++)
                _teamCount.Add(0);
        }

        public void UpdateObjectsCount(Planets.Base planet ,Planets.Team oldTeam, Planets.Team newTeam)
        {
            _teamCount[(int) oldTeam]--;
            _teamCount[(int) newTeam]++;

            TeamCountUpdated?.Invoke(_teamCount);
            CheckGameOver();
        }

        public void Clear()
        {
            for (int i = 0; i < _teamCount.Count; i++)
                _teamCount[i] = 0;
        }

        public void FillTeamCount(List<Planets.Base> allPlanets)
        {
            foreach (Planets.Base planet in allPlanets)
            {
                int team = (int) planet.Team;
                _teamCount[team]++;
            }
            TeamCountUpdated?.Invoke(_teamCount);
        }

        private void CheckGameOver()
        {
            bool noneBlue = _teamCount[(int)Planets.Team.Blue]==0;
            bool noneRed = _teamCount[(int)Planets.Team.Red]==0;
            if (noneBlue || noneRed)
            {
                bool isWin = noneRed;
                GameEnded?.Invoke(isWin);
            }
        }
    }
}