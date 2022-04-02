using System;

namespace _Application.Scripts.Skills
{
    public interface IBuffed
    {
        public bool IsBuffed { get; }
        void Buff(float percent);
        void UnBuff(float percent);
        event Action<Buildings.Base> Buffed;
        event Action<Buildings.Base> UnBuffed;
    }
}