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

        public GameObject InstantiateUI(string path, Canvas canvas)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, canvas.transform);
        }

        public T InstantiateUI<T>(string path, Canvas canvas) =>
            InstantiateUI(path, canvas).GetComponent<T>();
    }
}