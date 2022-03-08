using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.AssetManagement
{
    public interface IAssetProvider : IService
    {
        public GameObject Instantiate(string path);
        public GameObject InstantiateUI(string path, Canvas canvas);

    }
}