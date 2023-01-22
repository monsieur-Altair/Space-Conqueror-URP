using System.Collections.Generic;
using _Application.Scripts.Units;
using Pool_And_Particles;

namespace _Application.Scripts.Managers
{
    public class UnitSupervisor
    {
        private readonly GlobalPool _globalPool;
        private readonly List<BaseUnit> _units;

        public UnitSupervisor(GlobalPool globalPool)
        {
            _globalPool = globalPool;
            _units = new List<BaseUnit>();
            Enable();
        }

        public void DisableAll()
        {
            foreach (BaseUnit unit in _units)
            {
                Disable(unit);
            }
            _units.Clear();
        }

        private void Enable()
        {
            Units.BaseUnit.Launched += BaseOnLaunched;   
            Units.BaseUnit.Approached += BaseOnApproached;   
        }

        private void BaseOnApproached(BaseUnit unit)
        {
            _units.Remove(unit);
            Disable(unit);
        }

        private void Disable(BaseUnit unit)
        {
            _globalPool.Free(unit);
            unit.Stop();
        }

        private void BaseOnLaunched(BaseUnit unit)
        {
            _units.Add(unit);
        }
    }
}