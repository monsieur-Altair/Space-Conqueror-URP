using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Managers
{
    public class LevelManager : MonoBehaviourService
    {
        [SerializeField] 
        private NavMeshSurface navMeshSurfaceObj;
        
        [SerializeField] 
        private GameObject[] levels;

        private readonly Vector3 _instantiatePos = new Vector3(0, 0, 0);
        private GameObject _currentLevel;
        
        public int CurrentLevelNumber { get;  set; }
        
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
            _currentLevel.gameObject.transform.SetParent(transform.parent);
        }

        public void DeleteCurrentLevel()
        {
            StartCoroutine(DeleteAllLevel());
        }

        private IEnumerator DeleteAllLevel()
        {
            if (_currentLevel != null)
            {
                _currentLevel.gameObject.SetActive(false);
                Debug.Log("destroy" + _currentLevel.name);
                Destroy(_currentLevel.gameObject);
            }
            
            yield break;
        }
    }
}