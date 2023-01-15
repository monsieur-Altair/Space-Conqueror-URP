using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pool_And_Particles
{
    public class PooledParticleSystem : PooledBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] protected ParticleSystem[] _particleSystems = new ParticleSystem[0];
        
        public Transform Transform => _transform;

        public override void OnSpawnFromPool()
        {
            base.OnSpawnFromPool();

            if (_particleSystems == null || _particleSystems.Length == 0)
            {
                CalculateFreeTimeout();
            }
            
            Restart();
        }

        private void CalculateFreeTimeout()
        {
            // NOTE: вычисляем таймаут исходя из максимальной длительности системы частиц
            if (_particleSystems == null || _particleSystems.Length == 0)
            {
                _particleSystems = GetComponentsInChildren<ParticleSystem>();
            }

            if (_particleSystems.Length == 0)
            {
                throw new System.Exception($"{typeof(PooledParticleSystem)}: particle systems was not found in gameobject {nameof(gameObject)}");
            }

            FreeTimeout = ParticleUtility.CalculateMaxLifetime(_particleSystems);
        }

        public float GetFreeTimeout()
        {
            if (FreeTimeout == 0)
            {
                CalculateFreeTimeout();
            }
            return FreeTimeout;
        }

        public void Scale(Vector3 scale)
        {
            foreach (var system in _particleSystems)
            {
                system.transform.localScale = scale;
            }
        }

        public void Restart()
        {
            foreach (var system in _particleSystems)
            {
                system.Clear();
                system.Stop();

                system.randomSeed = (uint)Random.Range(int.MinValue, int.MaxValue);
                system.Play();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Calculate FreeTimeout")]
        private void BakeFreeTimeout()
        {
            CalculateFreeTimeout();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
