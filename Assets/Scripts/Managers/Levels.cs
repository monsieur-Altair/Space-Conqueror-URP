using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Managers
{
    public class Levels : MonoBehaviour
    {
        [SerializeField] private GameObject navMeshSurfaceObj;
        [SerializeField] private GameObject[] levels;
        
        public int currentLevelNumber;
        public static Levels Instance { get; private set; }
        
        private readonly Vector3 _instantiatePos = new Vector3(0,0,0);
        private NavMeshSurface _navMeshSurface;
        private GameObject _currentLevel;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            _navMeshSurface = navMeshSurfaceObj.GetComponent<NavMeshSurface>();
            if (navMeshSurfaceObj==null)
            {
                throw new MyException("cannot get nav mesh surface");
            }
        }

        public void SwitchToNextLevel()
        {
            StartCoroutine(DeleteAllLevel());
            currentLevelNumber++;
            if (currentLevelNumber == levels.Length)
                currentLevelNumber--;
        }


        private IEnumerator DeleteAllLevel()
        {
            Destroy(_currentLevel.gameObject);
            yield break;
        }

        public void RestartLevel()
        {
            if(_currentLevel!=null)
                Destroy(_currentLevel);
        }

        public void LoadCurrentLevel()
        {
            StartCoroutine(InstantiateLevel()) ;
        }

        public GameObject GetCurrentLay()
        {
            return _currentLevel;
        }

        private static int _callCount;
        
        public IEnumerator InstantiateLevel()
        {
            if(_callCount!=0)
                yield return StartCoroutine(DeleteAllLevel());
            _currentLevel = Instantiate(levels[currentLevelNumber], _instantiatePos, Quaternion.identity);
            _currentLevel.SetActive(true);
             Debug.Log("BAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKE");
            _navMeshSurface.BuildNavMesh();
            _currentLevel.gameObject.transform.SetParent(gameObject.transform.parent);
            _callCount ++;
        }
    }
}