using UnityEngine;

namespace Resources
{
    public abstract class Skill : ScriptableObject
    {
        public float cooldown;
        public int cost;
    }
}