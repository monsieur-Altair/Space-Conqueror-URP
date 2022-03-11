using UnityEngine;

namespace _Application.Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "new buff resource", menuName = "Resources/Buff resource")]
    public class Buff : Skill
    {
        public float buffPercent;
        public float duration;
    }
}