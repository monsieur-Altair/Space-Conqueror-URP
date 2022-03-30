using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace _Application.Scripts.Managers
{
    public class TeamProgressBar : MonoBehaviour
    {
        private const int FillBluePosition = 1;
        private const int FillRedPosition = 2;

        private Image _fillBlue;
        private Image _fillRed;

        private int _blue;
        private int _red;
        private int _maximum;

        private void Awake()
        {
            _fillBlue = transform.GetChild(FillBluePosition).GetComponent<Image>();
            _fillRed = transform.GetChild(FillRedPosition).GetComponent<Image>();
        }

        public void FillTeamCount(List<int> teamCount)
        {
            _blue = teamCount[0];
            _red = teamCount[1];
            _maximum = teamCount[0] + teamCount[1] + teamCount[2];
            _fillBlue.fillAmount = (float) _blue / _maximum;
            _fillRed.fillAmount = (float) _red / _maximum;
        }
    }
}
