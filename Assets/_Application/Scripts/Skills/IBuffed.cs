using System;

namespace _Application.Scripts.Skills
{
    public interface IBuffed
    {
        public bool IsBuffed { get; }
        void Buff(float percent);
        void UnBuff(float percent);
        event Action<Buildings.BaseBuilding> Buffed;
        event Action<Buildings.BaseBuilding> UnBuffed;
    }
}