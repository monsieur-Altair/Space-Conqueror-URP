using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Managers
{
    public class Levels : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navMeshSurfaceObj;
        [SerializeField] private GameObject[] levels;
        
        public int currentLevelNumber;
        public static Levels Instance { get; private set; }
        
        private readonly Vector3 _instantiatePos = new Vector3(0,0,0);
        private GameObject _currentLevel;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void SwitchToNextLevel()
        {
            StartCoroutine(DeleteAllLevel());
            currentLevelNumber++;
            if (currentLevelNumber == levels.Length)
                currentLevelNumber = 0;
        }

        public void RestartLevel()
        {
            if(_currentLevel!=null)
                Destroy(_currentLevel);
        }

        public GameObject GetCurrentLay() => 
            _currentLevel;

        public IEnumerator InstantiateLevel()
        {
            yield return StartCoroutine(DeleteAllLevel());
            _currentLevel = Instantiate(levels[currentLevelNumber], _instantiatePos, Quaternion.identity);
            _currentLevel.SetActive(true);
            navMeshSurfaceObj.BuildNavMesh();
            _currentLevel.gameObject.transform.SetParent(gameObject.transform.parent);
        }

        private IEnumerator DeleteAllLevel()
        {
            if(_currentLevel!=null)
                Destroy(_currentLevel.gameObject);
                //_currentLevel.gameObject.SetActive(false);
            yield break;
        }
    }
}