using UnityEngine;

namespace _Application.Scripts.Scriptables
{
    public abstract class Skill : ScriptableObject
    {
        public float cooldown;
        public int cost;
    }
}