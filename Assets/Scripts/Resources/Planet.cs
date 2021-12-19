using UnityEngine;

namespace Resources
{    
    [CreateAssetMenu (fileName = "new planet resource",menuName = "Resources/Planet Resource")]
    public class Planet : ScriptableObject
    {
        public int maxCount;
        public float produceCount;
        public float produceTime;
        public float defense;
    }
}