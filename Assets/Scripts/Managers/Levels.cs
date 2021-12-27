using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
//using Buildeerrr= UnityEditor.AI.NavMeshBuilder;

namespace Managers
{
    public class Levels : MonoBehaviour
    {
        [SerializeField] private GameObject[] levels;
        public int currentLevelNumber;
        private readonly Vector3 _instantiatePos = new Vector3(0,0,0);
        [SerializeField] private GameObject navMeshSurfaceObj;
        //[SerializeField] private NavMeshData Data;
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

            /*Data = GetComponent<NavMeshData>();
            if (Data == null)
                throw new MyException("cannot get data");*/
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
            //InstantiateLevel();
        }

        public void LoadCurrentLevel()
        {
            StartCoroutine(InstantiateLevel()) ;
        }

        public GameObject GetCurrentLay()
        {
            return _currentLevel;
        }

        private static int callCount = 0;
        
        public IEnumerator InstantiateLevel()
        {
            if(callCount!=0)
                yield return StartCoroutine(DeleteAllLevel());
            _currentLevel = Instantiate(levels[currentLevelNumber], _instantiatePos, Quaternion.identity);
            _currentLevel.SetActive(true);
             Debug.Log("BAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKE");
            _navMeshSurface.BuildNavMesh();
            _currentLevel.gameObject.transform.SetParent(gameObject.transform.parent);
            callCount ++;
        }
    }
}