using System;
using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.SavedData;

namespace _Application.Scripts.Managers
{
    public class TeamManager
    {
        public static event Action<List<int>> TeamCountUpdated = delegate {  };
        public static event Action<bool> GameEnded = delegate {  };
        
        private readonly int _teamNumber = Enum.GetNames(typeof(Team)).Length;
        private readonly List<int> _teamCount;
        private readonly ProgressService _progressService;


        public TeamManager(ProgressService progressService)
        {
            _progressService = progressService;
            _teamCount = new List<int>(_teamNumber);
            for (int i = 0; i < _teamNumber;i++)
                _teamCount.Add(0);
        }

        public void UpdateObjectsCount(Base building, Team oldTeam, Team newTeam)
        {
            _teamCount[(int) oldTeam]--;
            _teamCount[(int) newTeam]++;

            CheckForStatistic(oldTeam, newTeam);
            
            TeamCountUpdated(_teamCount);
            CheckGameOver();
        }

        private void CheckForStatistic(Team oldTeam, Team newTeam)
        {
            if (oldTeam == Team.Blue)
                _progressService.PlayerProgress.Statistic.MissedBuildings++;
            if (newTeam == Team.Blue)
                _progressService.PlayerProgress.Statistic.ConqueredBuildings++;
        }

        public void Clear()
        {
            for (int i = 0; i < _teamCount.Count; i++)
                _teamCount[i] = 0;
        }

        public void FillTeamCount(List<Base> allBuildings)
        {
            foreach (Base building in allBuildings)
            {
                int team = (int) building.Team;
                _teamCount[team]++;
            }
            TeamCountUpdated(_teamCount);
        }

        private void CheckGameOver()
        {
            bool noneBlue = _teamCount[(int) Team.Blue] == 0;
            bool noneRed = _teamCount[(int) Team.Red] == 0;
            if (noneBlue || noneRed)
            {
                bool isWin = noneRed;
                GameEnded(isWin);
            }
        }
    }
}