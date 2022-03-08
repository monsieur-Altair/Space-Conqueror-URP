using System.Collections.Generic;
using _Application.Scripts.Managers;
using _Application.Scripts.Planets;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace mainMenu
{
    public class ProgressBar : MonoBehaviour
    {
        private float _currentScience;
        private static int maximumScience = 100;
        public Image progressBerScience;
    
        public Image progressBerPlanetsBlue;
        public Image progressBerPlanetsRed;
        private int _blue;
        private int _red;
        private int _maximum;

        private void LateUpdate()
        {
            GetCurrentFillPlanets();
            GetCurrentFillScience();
        }

        private void GetCurrentFillPlanets()
        {
            List<int> planets = Main.Instance.ObjectsCount;
            _blue = planets[0];
            _red = planets[1];
            _maximum = planets[0] + planets[1] + planets[2];
            progressBerPlanetsBlue.fillAmount = (float) _blue / _maximum;
            progressBerPlanetsRed.fillAmount = (float) _red / _maximum;
        }

        private void GetCurrentFillScience()
        {
            _currentScience = Scientific.ScientificCount;
            float fill = _currentScience / maximumScience;
            progressBerScience.fillAmount = fill;
        }
    }
}
