using UnityEngine;

namespace _Application.Scripts.Control
{
    public class StandaloneInput : BaseInput
    {
        public override void HandleInput()
        {
            if (SkillController.SelectedSkillName != SkillName.None)
                AffectToSkills();
            else
                AffectToBuilding();
        }

        protected override void AffectToBuilding()
        {
            Vector3 pos = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
                BuildingsController.HandleRelease(pos);
            else if (Input.GetMouseButtonDown(0))
                BuildingsController.HandleClick(pos);
            else if(Input.GetMouseButton(0)) 
                BuildingsController.HandleMultipleSelection(pos);
        }

        protected override void AffectToSkills()
        {
            if (Input.GetMouseButtonUp(0)) 
                SkillController.ApplySkill(Input.mousePosition);
        }
    }
}