using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(fileName = "new buff resource", menuName = "Resources/Buff resource")]
    public class Buff : Skill
    {
        public float buffPercent;
    }
}