using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

namespace Managers
{
    public class Levels : MonoBehaviour
    {
        [SerializeField] private GameObject[] levels;
        public int currentLevelNumber;
        private readonly Vector3 _instantiatePos = new Vector3(0,0,0);
        [SerializeField] private GameObject navMeshSurfaceObj;
        private NavMeshSurface _navMeshSurface;
        private GameObject _currentLevel;
        public static Levels Instance { get; private set; }

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
            Destroy(_currentLevel.gameObject);
            //_navMeshSurface.BuildNavMesh();
            currentLevelNumber++;
            if (currentLevelNumber == levels.Length)
                currentLevelNumber--;
            /*if (currentLevelNumber <= levels.Length)
            {
                InstantiateLevel();
            }*/
        }

        public void RestartLevel()
        {
            if(_currentLevel!=null)
                Destroy(_currentLevel);
            //InstantiateLevel();
        }

        public void LoadCurrentLevel()
        {
            InstantiateLevel();
        }

        public GameObject GetCurrentLay()
        {
            return _currentLevel;
        }

        private void InstantiateLevel()
        {
            _currentLevel = Instantiate(levels[currentLevelNumber], _instantiatePos, Quaternion.identity);
            _currentLevel.SetActive(true);
            _navMeshSurface.BuildNavMesh();
            _currentLevel.gameObject.transform.SetParent(gameObject.transform.parent);
        }

        public void BakeNavMesh()
        {
            _navMeshSurface.BuildNavMesh();
        }
        
        /*public void OnEnable()
        {
            Debug.Log("enable lvls");
            
        }*/
    }
}