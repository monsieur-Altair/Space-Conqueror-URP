using System.Collections.Generic;
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

        private void Update()
        {
            GetCurrentFillPlanets();
            GetCurrentFillScience();
        }

        private void GetCurrentFillPlanets()
        {
            List<int> planets = Managers.Main.ObjectsCount;
            _blue = planets[0];
            _red = planets[1];
            _maximum = planets[0] + planets[1] + planets[2];
            progressBerPlanetsBlue.fillAmount = (float) _blue / _maximum;
            progressBerPlanetsRed.fillAmount = (float) _red / _maximum;
        }

        private void GetCurrentFillScience()
        {
            _currentScience = Planets.Scientific.ScientificCount;
            float fill = (float) _currentScience / (float) maximumScience;
            progressBerScience.fillAmount = fill;
        }
    }
}
