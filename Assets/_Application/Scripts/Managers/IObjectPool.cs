using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public interface IObjectPool : IService
    {
        GameObject GetObject(PoolObjectType type, Vector3 position, Quaternion rotation);
        void DisableAllUnitsInScene();
        void Init();
    }
}