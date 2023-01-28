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
            BaseUnit.Launched += BaseOnLaunched;   
            BaseUnit.Approached += BaseOnApproached;   
        }

        private void BaseOnApproached(BaseUnit unit)
        {
            _units.Remove(unit);
            Disable(unit);
        }

        private void Disable(BaseUnit unit)
        {
            unit.Stop();
            _globalPool.Free(unit);
        }

        private void BaseOnLaunched(BaseUnit unit)
        {
            _units.Add(unit);
        }
    }
}