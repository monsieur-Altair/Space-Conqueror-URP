using UnityEngine;

namespace Skills
{
    public interface ISkill
    {
        void ExecuteForPlayer(Vector3 pos);
        void ExecuteForAI(Planets.Base planet);
    }
}