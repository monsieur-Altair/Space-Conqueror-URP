using TMPro;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace _Application.Scripts.Managers
{
    public class ManaBar : MonoBehaviour
    {
        [SerializeField]
        private ProceduralImage _fill;

        [SerializeField] private TextMeshProUGUI _manaText;
        
        public void FillManaCount(float manaCount, int max)
        {
            _fill.fillAmount = manaCount / max;
            _manaText.text= $"{Mathf.Round(manaCount)}/{max}";
        }
    }
}
