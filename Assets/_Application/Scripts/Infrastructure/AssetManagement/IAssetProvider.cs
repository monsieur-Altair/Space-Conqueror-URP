using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.AssetManagement
{
    public interface IAssetProvider : IService
    {
        public GameObject Instantiate(string path);
        public T Instantiate<T>(string path);
        public GameObject InstantiateUI(string path, Canvas canvas);
        public T InstantiateUI<T>(string path, Canvas canvas);
        T InstantiateScriptable<T>(string path) where T : ScriptableObject;
    }
}