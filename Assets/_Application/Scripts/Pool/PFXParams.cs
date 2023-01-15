using System;
using UnityEngine;

namespace Pool_And_Particles
{
    [Serializable]
    public class PFXParams
    {
        public PooledParticleSystem Particle;
        public bool UseLocalPosition = true;
        public Vector3 Position;
        public bool UseLocalRotation;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;

        public PooledParticleSystem Get(Transform parent, GlobalPool globalPool)
        {
            var position = UseLocalPosition ? parent.TransformPoint(Position) : Position;
            var rotation = UseLocalRotation ? parent.rotation * Quaternion.Euler(Rotation) : Quaternion.Euler(Rotation);
            return globalPool.Get(Particle, 
                parent: parent,
                position: position, 
                rotation: rotation,
                localScale: Scale);
        }

        public PooledParticleSystem GetWithoutParent(Transform parent, GlobalPool globalPool)
        {
            var localScale = parent.localScale;
            var scale = new Vector3(Scale.x * localScale.x,
                Scale.y * localScale.y,
                Scale.z * localScale.z);
            var position = UseLocalPosition ? parent.TransformPoint(Position) : Position;
            var rotation = UseLocalRotation ? parent.rotation * Quaternion.Euler(Rotation) : Quaternion.Euler(Rotation);
            return globalPool.Get(Particle,
                position: position,
                rotation: rotation,
                localScale: scale);
        }
        
        public PooledParticleSystem GetWorldSpace(Vector3 position, Quaternion rotation, Vector3 scale, GlobalPool globalPool)
        {
            scale = new Vector3(Scale.x * scale.x,
                Scale.y * scale.y,
                Scale.z * scale.z);
            return globalPool.Get(Particle,
                position: position + Position,
                rotation: rotation * Quaternion.Euler(Rotation),
                localScale: scale);
        }
    }
}