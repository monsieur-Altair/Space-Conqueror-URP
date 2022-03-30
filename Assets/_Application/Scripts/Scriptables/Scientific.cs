using UnityEngine;

namespace _Application.Scripts.Scriptables
{
    [CreateAssetMenu (fileName = "new scientific resource",menuName = "Resources/Scientific Resource")]
    public class Scientific:ScriptableObject
    {
        public int maxCount;
        public float produceCount;
        public float produceTime;

    }
}