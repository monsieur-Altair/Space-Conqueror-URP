using System;
using DG.Tweening;
using UnityEngine;

namespace _Application.Scripts.UI
{
    public class TargetedHand : MonoBehaviour
    {
        private Tween _handAnimation;
        private Vector3 _endPos;
        
        [SerializeField]
        private float _duration = 2.0f;
        
        [SerializeField]
        private float _distance = 100.0f;

        private void Awake() => 
            _endPos = transform.localPosition + transform.up * _distance;

        private void OnEnable()
        {
            _handAnimation = transform
                .DOLocalMove(_endPos, _duration)
                .SetEase(Ease.OutBack)
                .SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            if (_handAnimation != null)
            {
                _handAnimation.Kill();
                _handAnimation = null;
            }
        }
    }
}