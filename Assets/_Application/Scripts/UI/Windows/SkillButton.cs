using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    public class SkillButton : MonoBehaviour
    {
        public event Action<SkillButton> Clicked = delegate { };

        [SerializeField] private Image _mask;
        [SerializeField] private Button _button;

        public Button Button => _button;
        public Image Mask => _mask;

        public void OnOpened()
        {
            _button.onClick.AddListener(OnClicked);
            Color color = _mask.color;
            _mask.color = new Color(color.r, color.g, color.b, 0.0f);
        }

        public void OnClosed()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            Clicked(this);
        }
    }
}