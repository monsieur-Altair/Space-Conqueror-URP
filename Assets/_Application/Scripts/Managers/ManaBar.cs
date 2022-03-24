using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace _Application.Scripts.Managers
{
    public class ManaBar : MonoBehaviour
    {
        private const int FillPosition = 0;
        private Image _fill;
        
        private void Awake() => 
            _fill = transform.GetChild(FillPosition).GetComponent<Image>();

        public void FillManaCount(float manaCount, int max) =>
            _fill.fillAmount = manaCount / max;
    }
}
