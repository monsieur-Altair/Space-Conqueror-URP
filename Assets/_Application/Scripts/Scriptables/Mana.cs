using UnityEngine;

namespace _Application.Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "new scientific resource", menuName = "Resources/Mana Resource")]
    public class Mana : ScriptableObject
    {
        public int maxCount;
        public float produceCount;
        public float produceTime;
    }
}