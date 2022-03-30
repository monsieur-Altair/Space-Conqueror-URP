using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace _Application.Scripts.Managers
{
    public class ScientificBar : MonoBehaviour
    {
        private const int FillPosition = 0;
        private Image _fill;
        
        private void Awake() => 
            _fill = transform.GetChild(FillPosition).GetComponent<Image>();

        public void FillScientificCount(float scientificCount, int max) =>
            _fill.fillAmount = scientificCount / max;
    }
}
