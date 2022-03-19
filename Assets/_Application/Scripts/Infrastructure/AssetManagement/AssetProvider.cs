using UnityEngine;

namespace _Application.Scripts.Infrastructure.AssetManagement
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

        public GameObject InstantiateUI(string path, Canvas canvas)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, canvas.transform);
        }

        public T InstantiateUI<T>(string path, Canvas canvas) => 
            InstantiateUI(path,canvas).GetComponent<T>();
    }
}