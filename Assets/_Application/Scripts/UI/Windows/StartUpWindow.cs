using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class StartUpWindow : Window
    {
        [SerializeField] 
        private Button _playButton;

        [SerializeField] 
        private Button _toStatsButton;

        public Button PlayButton => _playButton;
        public Button ToStatsButton => _toStatsButton;
    }
}