using UnityEngine;

namespace _Application.Scripts.Infrastructure
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] 
        private GameBootstrapper _bootstrapper;

        private void Awake()
        {
            if (FindObjectOfType<GameBootstrapper>() == null)
            {
                Instantiate(_bootstrapper);
                Destroy(gameObject);
            }
        }
    }
}