using System;
using UnityEngine;

namespace Pool_And_Particles
{
    public static class ParticleUtility
    {
        public static float CalculateMaxLifetime(GameObject particlesParent)
        {
            return CalculateMaxLifetime(particlesParent.GetComponentsInChildren<ParticleSystem>());
        }

        public static float CalculateMaxLifetime(ParticleSystem[] particleSystems)
        {
            if (particleSystems == null || particleSystems.Length == 0)
            {
                throw new Exception($"{typeof(ParticleUtility)} CalculateMaxLifetime: particle systems is null or empty");
            }

            ParticleSystem maxLifeTimeParticleSystem = null;
            float maxLifeTime = 0f;
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem.main.startLifetime.constantMax > maxLifeTime)
                {
                    maxLifeTime = particleSystem.main.startLifetime.constantMax;
                    maxLifeTimeParticleSystem = particleSystem;
                }
            }
            return GetMaxLifetime(maxLifeTimeParticleSystem);
        }

        public static float GetMaxLifetime(ParticleSystem particleSystem)
        {
            return particleSystem.main.startLifetime.constantMax;
        }
    }
}