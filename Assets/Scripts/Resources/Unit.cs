using UnityEngine;

namespace Resources
{
    [CreateAssetMenu (fileName = "new unit",menuName = "Resources/Unit")]
    public class Unit:ScriptableObject
    {
        public float damage;
        public float speed;
    }
}