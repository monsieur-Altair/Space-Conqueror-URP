using UnityEngine;

namespace _Application.Scripts.Control
{
    public class StandaloneInput : BaseInput
    {
        public override void HandleInput()
        {
            if (_skillController.IsSelectedSkill)
                AffectToSkills();
            else
                AffectToPlanet();
        }

        protected override void AffectToPlanet()
        {
            Vector3 pos = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                _planetController.HandleRelease(pos);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _planetController.HandleClick(pos);
            }
            else if(Input.GetMouseButton(0))
            {
                _planetController.HandleMultipleSelection(pos);
            }
        }

        protected override void AffectToSkills()
        {
            if (Input.GetMouseButtonUp(0))
            {
                _skillController.ApplySkill(Input.mousePosition);
            }
        }
    }
}