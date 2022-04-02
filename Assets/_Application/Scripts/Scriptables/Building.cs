using UnityEngine;

namespace _Application.Scripts.Scriptables
{    
    [CreateAssetMenu (fileName = "new planet resource",menuName = "Resources/Building Resource")]
    public class Building : ScriptableObject
    {
        public int maxCount;
        public float produceCount;
        public float produceTime;
        public float defense;
        public float reducingSpeed;
    }
}