
using UnityEngine;

namespace Resources
{    
    [CreateAssetMenu(fileName = "new acid resource", menuName = "Resources/Acid resource")]
    public class Acid : Skill
    {
        public float duration;
        public float damage;
        public int hitCount;
    }
}