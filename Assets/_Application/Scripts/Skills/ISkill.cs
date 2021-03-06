using UnityEngine;

namespace _Application.Scripts.Skills
{
    public interface ISkill
    {
        void ExecuteForPlayer(Vector3 pos);
        void ExecuteForAI(Buildings.Base planet);
    }
}