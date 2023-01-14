using DG.Tweening;
using UnityEngine;

namespace _Application.Scripts.UI.Windows.Tutorial
{
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public class AnimatedWindow : Window
    {
        private const float MaxAlpha = 1.0f;
        
        protected float _endAlphaValue = 0.0f;
        protected float _duration = 5.0f;
        
        private Animator _animator;
        private CanvasGroup _canvasGroup;

        public override void GetDependencies()
        {
            base.GetDependencies();

            _animator = GetComponent<Animator>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnOpened() => 
            StartFadeOutAnimation();

        protected virtual void StartFadeOutAnimation()
        {
            _canvasGroup.alpha = MaxAlpha;
            _canvasGroup
                .DOFade(_endAlphaValue, _duration)
                .SetEase(Ease.InCirc)
                .OnComplete(OnFadeCompleted);
        }

        private void OnFadeCompleted()
        {
            if(IsOpened)   
                Close();
        }
    }
}