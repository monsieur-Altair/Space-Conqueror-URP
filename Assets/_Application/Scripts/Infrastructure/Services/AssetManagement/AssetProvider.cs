using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public GameObject Instantiate(string path)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }

        public T Instantiate<T>(string path) =>
            Instantiate(path).GetComponent<T>();

        public T InstantiateScriptable<T>(string path) where T : ScriptableObject =>
            Resources.Load<T>(path);
    }
}