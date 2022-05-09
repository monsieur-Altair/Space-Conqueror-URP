using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public interface ISkillController : IService
    {
        Skills.Call Call { get; }
        Skills.Buff Buff { get; }
        Skills.Acid Acid { get; }
        Skills.Ice Ice { get; }
        bool IsSkillNotSelected { get; }
        SkillName SelectedSkillName { get; }
        void ApplySkill(Vector3 position);
        void ReloadSkills();
        void RefreshSkills();
        void SetSelectedSkill(int index);
        void ClearSelectedSkill();
    }
}