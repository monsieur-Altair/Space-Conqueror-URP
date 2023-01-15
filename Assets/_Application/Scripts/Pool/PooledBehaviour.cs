using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool_And_Particles
{
    public class PooledBehaviour : MonoBehaviour
    {
        public static event Action<PooledBehaviour> Spawned = delegate { };
        public event Action<PooledBehaviour> Returned = delegate { };

        public event Action InstanceSpawned = null;
        public event Action InstanceReturned = null;

        [SerializeField] private float _freeTimeout = 0f;
        [SerializeField] private bool _freeAfterTime = true;
        
        protected GlobalPool GlobalPool;


        private Coroutine _freeTimeoutTween = null;

        private Dictionary<string, object> _alteringData = null;
        private Dictionary<string, object> _preparationData = null;

        private bool _isPrepared = false;

        public float FreeTimeout
        {
            get { return _freeTimeout; }

            set
            {
                _freeTimeout = value;

                if (gameObject.activeInHierarchy)
                {
                    HandleFreeTimeoutChanged();
                }
            }
        }

        public void SetPool(GlobalPool globalPool)
        {
            GlobalPool = globalPool;
        }
        
        public void Prepare(Dictionary<string, object> data)
        {
            if (!_isPrepared)
            {
                _preparationData = data;
                OnPreparePool();
                _isPrepared = true;
            }
        }

        protected virtual void OnPreparePool()
        {
        }

        public virtual void OnSpawnFromPool()
        {   
            if (_freeAfterTime)
                HandleFreeTimeoutChanged();

            if (InstanceSpawned != null)
            {
                InstanceSpawned();
                InstanceSpawned = null;
            }

            Spawned(this);
        }

        public virtual bool BeforeReturnToPool()
        {
            return true;
        }

        public virtual void OnReturnToPool()
        {
            if (InstanceReturned != null)
            {
                InstanceReturned();
                InstanceReturned = null;
            }

            Returned(this);
        }

        private void HandleFreeTimeoutChanged()
        {
            if (_freeTimeoutTween != null)
            {
                StopCoroutine(_freeTimeoutTween);
                _freeTimeoutTween = null;
            }

            if (_freeTimeout > 0.01f)
            {
             //   _freeTimeoutTween = Extensionss.Wait(_freeTimeout).OnComplete(() => _pool.TryFree(this));
                _freeTimeoutTween = StartCoroutine(InvokeWithDelay(_freeTimeout, () => { GlobalPool.TryFree(this); }));
            }
        }

        public void SetData(Dictionary<string, object> data)
        {
            this._alteringData = data;
        }

        public void ClearData()
        {
            _alteringData = null;
        }

        protected T GetPrepareDataValue<T>(string itemKey, T defaultValue = default)
        {
            return GetDataValue<T>(itemKey, defaultValue, _preparationData);
        }

        protected T GetDataValue<T>(string itemKey, T defaultValue = default, Dictionary<string, object> forcedData = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(forcedData ?? this._alteringData);

            if (data == null || data.Count == 0)
            {
                return defaultValue;
            }

            if (!data.TryGetValue(itemKey, out object itemObject))
            {
                return defaultValue;
            }

            if (itemObject is T value)
            {
                return value;
            }

            return defaultValue;
        }

        private IEnumerator InvokeWithDelay(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }
}