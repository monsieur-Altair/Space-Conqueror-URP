using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class StartUpWindow : Window
    {
        [SerializeField] 
        private Button _playButton;
        
        public Button PlayButton => _playButton;
    }
}