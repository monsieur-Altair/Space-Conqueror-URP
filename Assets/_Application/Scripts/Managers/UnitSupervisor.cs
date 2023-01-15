using System.Collections.Generic;
using _Application.Scripts.Units;
using Pool_And_Particles;

namespace _Application.Scripts.Managers
{
    public class UnitSupervisor
    {
        private readonly GlobalPool _globalPool;
        private readonly List<Base> _units;

        public UnitSupervisor(GlobalPool globalPool)
        {
            _globalPool = globalPool;
            _units = new List<Base>();
            Enable();
        }

        public void DisableAll()
        {
            foreach (Base unit in _units)
            {
                Disable(unit);
            }
            _units.Clear();
        }

        private void Enable()
        {
            Units.Base.Launched += BaseOnLaunched;   
            Units.Base.Approached += BaseOnApproached;   
        }

        private void BaseOnApproached(Base unit)
        {
            _units.Remove(unit);
            Disable(unit);
        }

        private void Disable(Base unit)
        {
            _globalPool.Free(unit);
            unit.Stop();
        }

        private void BaseOnLaunched(Base unit)
        {
            _units.Add(unit);
        }
    }
}