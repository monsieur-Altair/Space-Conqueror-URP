using System;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public class MobileInput : BaseInput
    {
        private Touch _touch;
        
        public override void HandleInput()
        {
            if (Input.touchCount > 0)
            {
                _touch = Input.GetTouch(0);
                if (_skillController.IsSelectedSkill)
                    AffectToSkills();
                else
                    AffectToPlanet();
            }
        }

        protected override void AffectToSkills()
        {
            switch (_touch.phase)
            {
                case TouchPhase.Ended:
                {
                    _skillController.ApplySkill(_touch.position);
                    break;
                }
            }
        }
        
        protected override void AffectToPlanet()
        {
        
            Vector3 position = _touch.position;
            switch (_touch.phase)
            {
                case TouchPhase.Began:
                {
                    _planetController.HandleClick(position);
                    break;
                }
                case TouchPhase.Ended:
                {
                    _planetController.HandleRelease(position);
                    break;
                }
                case TouchPhase.Moved:
                {
                    _planetController.HandleMultipleSelection(position);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}