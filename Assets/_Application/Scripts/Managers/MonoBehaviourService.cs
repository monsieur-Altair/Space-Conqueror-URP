using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public abstract class MonoBehaviourService: MonoBehaviour, IService
    {
        public virtual void Init()
        {
            DontDestroyOnLoad(this);
        }
    }
}