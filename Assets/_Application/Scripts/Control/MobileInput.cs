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
                if (SkillController.SelectedSkillName != SkillName.None)
                    AffectToSkills();
                else
                    AffectToBuilding();
            }
        }

        protected override void AffectToSkills()
        {
            switch (_touch.phase)
            {
                case TouchPhase.Ended:
                {
                    SkillController.ApplySkill(_touch.position);
                    break;
                }
            }
        }
        
        protected override void AffectToBuilding()
        {
        
            Vector3 position = _touch.position;
            switch (_touch.phase)
            {
                case TouchPhase.Began:
                {
                    BuildingsController.HandleClick(position);
                    break;
                }
                case TouchPhase.Ended:
                {
                    BuildingsController.HandleRelease(position);
                    break;
                }
                case TouchPhase.Moved:
                {
                    BuildingsController.HandleMultipleSelection(position);
                    break;
                }
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}