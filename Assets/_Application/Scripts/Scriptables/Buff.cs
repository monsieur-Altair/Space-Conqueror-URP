using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "new buff resource", menuName = "Resources/Buff resource")]
    public class Buff : Skill
    {
        public float buffPercent;
    }
}