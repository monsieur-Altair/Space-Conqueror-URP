using UnityEngine;

namespace _Application.Scripts.Control
{
    public class StandaloneInput : BaseInput
    {
        public override void HandleInput()
        {
            if (_skillController.SelectedSkillName != SkillName.None)
                AffectToSkills();
            else
                AffectToBuilding();
        }

        protected override void AffectToBuilding()
        {
            Vector3 pos = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                _buildingsController.HandleRelease(pos);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _buildingsController.HandleClick(pos);
            }
            else if(Input.GetMouseButton(0))
            {
                _buildingsController.HandleMultipleSelection(pos);
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