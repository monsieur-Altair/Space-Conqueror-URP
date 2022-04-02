using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Managers
{
    public class Levels : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navMeshSurfaceObj;
        [SerializeField] private GameObject[] levels;

        public int CurrentLevelNumber { get;  set; }
        public static Levels Instance { get; private set; }

        private readonly Vector3 _instantiatePos = new Vector3(0, 0, 0);
        private GameObject _currentLevel;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            //navMeshSurfaceObj = FindObjectOfType<NavMeshSurface>();
        }

        public void SwitchToNextLevel()
        {
            StartCoroutine(DeleteAllLevel());
            CurrentLevelNumber++;
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
            if (CurrentLevelNumber >= levels.Length)
                CurrentLevelNumber = 0;
            
            yield return StartCoroutine(DeleteAllLevel());

            _currentLevel = Instantiate(levels[CurrentLevelNumber], _instantiatePos, Quaternion.identity);
            _currentLevel.SetActive(true);
            navMeshSurfaceObj.BuildNavMesh();
            //yield return new WaitForSeconds(0.3f);
            _currentLevel.gameObject.transform.SetParent(gameObject.transform.parent);
        }

        private IEnumerator DeleteAllLevel()
        {
            if(_currentLevel!=null)
                Destroy(_currentLevel.gameObject);
            yield break;
        }
    }
}