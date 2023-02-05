using Pool_And_Particles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace _Application.Scripts.UI
{
    public class Counter : PooledBehaviour
    {
        [SerializeField] private ProceduralImage _backImage;
        [SerializeField] private TextMeshProUGUI _tmp;
        [SerializeField] private RectTransform _rectTransform;

        public void SetColors(Color foreground, Color background)
        {
            _tmp.color = foreground;
            _backImage.color = background;
        }

        public void SetText(string str)
        {
            _tmp.text = str;
        }

        public void SetAnchorPos(Vector2 anchorPos)
        {
            _rectTransform.anchoredPosition = anchorPos;
        }
    }
}