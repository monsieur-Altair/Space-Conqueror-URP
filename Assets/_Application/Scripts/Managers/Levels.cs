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
                currentLevelNumber--;
        }

        public void RestartLevel()
        {
            if(_currentLevel!=null)
                Destroy(_currentLevel);
        }

        public GameObject GetCurrentLay() => 
            _currentLevel;

        private static int _callCount;

        public IEnumerator InstantiateLevel()
        {
            if(_callCount!=0)
                yield return StartCoroutine(DeleteAllLevel());
            _currentLevel = Instantiate(levels[currentLevelNumber], _instantiatePos, Quaternion.identity);
            _currentLevel.SetActive(true);
             //Debug.Log("BAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKE");
            navMeshSurfaceObj.BuildNavMesh();
            _currentLevel.gameObject.transform.SetParent(gameObject.transform.parent);
            _callCount ++;
        }

        private IEnumerator DeleteAllLevel()
        {
            Destroy(_currentLevel.gameObject);
            yield break;
        }
    }
}