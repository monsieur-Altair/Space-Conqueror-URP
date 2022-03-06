using UnityEngine;

namespace Scriptables
{
    public abstract class Skill : ScriptableObject
    {
        public float cooldown;
        public int cost;
    }
}