using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using Image = UnityEngine.UI.Image;

namespace _Application.Scripts.Managers
{
    public class TeamProgressBar : MonoBehaviour
    {
        [SerializeField] private ProceduralImage _fillBlue;
        [SerializeField] private ProceduralImage _fillRed;

        public void FillTeamCount(List<int> teamCount)
        {
            int blue = teamCount[0];
            int red = teamCount[1];
            int maximum = teamCount[0] + teamCount[1] + teamCount[2];
            _fillBlue.fillAmount = (float) blue / maximum;
            _fillRed.fillAmount = (float) red / maximum;
        }
    }
}
