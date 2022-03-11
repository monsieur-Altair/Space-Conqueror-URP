using System;

namespace _Application.Scripts.Skills
{
    public interface IBuffed
    {
        public bool IsBuffed { get; set; }
        void Buff(float percent);
        void UnBuff(float percent);
        event Action<Planets.Base> Buffed;
        event Action<Planets.Base> UnBuffed;
    }
}