using UnityEngine;

namespace _Application.Scripts.Scriptables
{
    [CreateAssetMenu (fileName = "new unit",menuName = "Resources/Unit")]
    public class Unit:ScriptableObject
    {
        public float damage;
        public float speed;
    }
}