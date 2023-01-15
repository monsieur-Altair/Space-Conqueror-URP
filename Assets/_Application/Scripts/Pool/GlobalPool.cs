using System;
using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pool_And_Particles
{
    [Serializable]
    public class GlobalPool : IService
    {
        private class QueueEntry
        {
            public readonly PooledBehaviour Object;
            public readonly bool IsNew;

            public QueueEntry(PooledBehaviour obj, bool isNew)
            {
                Object = obj;
                IsNew = isNew;
            }
        }

        // Key = Prefab, Value = free objects
        private readonly Dictionary<PooledBehaviour, Queue<QueueEntry>> _freeObjects =
            new Dictionary<PooledBehaviour, Queue<QueueEntry>>();

        // Key = busy object, Value = prefab
        private readonly Dictionary<PooledBehaviour, PooledBehaviour> _busyObjects =
            new Dictionary<PooledBehaviour, PooledBehaviour>();

        private readonly int _capacity;
        private CoreConfig _coreConfig;

        public static event Action<PooledBehaviour, int> EntryInstantiated = delegate { };

        private Transform DefaultParent { get; }

        public GlobalPool(CoreConfig coreConfig, Transform defaultParent)
        {
            _coreConfig = coreConfig;
            _capacity = 500;
            DefaultParent = defaultParent;
        }

        public T Get<T>(T prefab, Vector3? position = null, Quaternion? rotation = null, Vector3? localPosition = null,
            Quaternion? localRotation = null, Vector3? localScale = null, Transform parent = null,
            Dictionary<string, object> data = null,
            bool isReusable = false)
            where T : PooledBehaviour
        {
            return Get(prefab, out bool unused, position, rotation, localPosition, localRotation, localScale, parent,
                data);
        }

        public T Get<T>(T prefab, out bool isNew, Vector3? position = null, Quaternion? rotation = null,
            Vector3? localPosition = null, Quaternion? localRotation = null, Vector3? localScale = null,
            Transform parent = null,
            Dictionary<string, object> data = null, bool isReusable = false)
            where T : PooledBehaviour
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            isNew = false;

            T result = null;
            if (_freeObjects.TryGetValue(prefab, out Queue<QueueEntry> freeQueue) && freeQueue.Count > 0)
            {
                var entry = freeQueue.Dequeue();
                result = (T) entry.Object;
                isNew = entry.IsNew;
            }

            parent = parent ?? DefaultParent;

            if (result == null)
            {
                if (isReusable)
                {
                    foreach (var pair in _busyObjects)
                    {
                        if (pair.Value == prefab && TryFree(pair.Key))
                        {
                            return Get(prefab, out bool dummy, position, rotation, localPosition, localRotation,
                                localScale,
                                parent, data);
                        }
                    }
                }
                else
                {
                    result = (T) Object.Instantiate(prefab, parent);
                    result.SetPool(this);
                    isNew = true;

                    EntryInstantiated(prefab, 1);
                }
            }

            // NOTE: добавил условие. По непонятным причинам, редактор Unity периодически повисает, в дебаге показывает эту строку, возможно
            // условие позволит избежать ошибки. Как правило у объектов в пуле родитель не меняется.
            if (result.transform.parent != parent)
            {
                result.transform.SetParent(parent, false);
            }

            if (position.HasValue)
            {
                result.transform.position = position.Value;
            }
            else if (localPosition.HasValue)
            {
                result.transform.localPosition = localPosition.Value;
            }

            if (rotation.HasValue)
            {
                result.transform.rotation = rotation.Value;
            }
            else if (localRotation.HasValue)
            {
                result.transform.localRotation = localRotation.Value;
            }

            if (localScale.HasValue)
            {
                if (result.transform.localScale != localScale)
                    result.transform.localScale = localScale.Value;
            }

            result.gameObject.SetActive(true);

            result.SetData(data);

            result.OnSpawnFromPool();

            _busyObjects[result] = prefab;

            return result;
        }

        public bool TryFree(PooledBehaviour obj, bool resetParent = true)
        {
            if (_busyObjects.ContainsKey(obj))
            {
                Free(obj, resetParent);
                return true;
            }

            return false;
        }

        public void Free(PooledBehaviour obj, bool resetParent = true)
        {
            if (_busyObjects.TryGetValue(obj, out PooledBehaviour prefab))
            {
                if (prefab != null)
                {
                    // NOTE: страхуемся от повторного вызова Free() для одного объекта, чтобы не пометить его, как дважды свободный
                    if (_busyObjects.Remove(obj))
                    {
                        Free(obj, prefab, resetParent);
                    }
                    else
                    {
                        Debug.LogWarningFormat(obj.gameObject, "[{0}] Free: Double free object: {1}", GetType().Name,
                            obj.name);
                    }
                }
            }
        }

        public void FreeAll(PooledBehaviour prefab = null, bool resetParent = true)
        {
            KeyValuePair<PooledBehaviour, PooledBehaviour>[] pairsToFree;
            if (prefab != null)
            {
                pairsToFree = _busyObjects.Where(obj => obj.Value == prefab).ToArray();
            }
            else
            {
                pairsToFree = _busyObjects.ToArray();
            }

            if (pairsToFree != null)
            {
                foreach (var pair in pairsToFree)
                {
                    Free(pair.Key, resetParent);
                }
            }
        }

        public void Prepare(PooledBehaviour prefab, int count, Transform parent = null,
            bool forceAddNew = true, Dictionary<string, object> data = null)
        {
            if (count <= 0)
            {
                Debug.LogErrorFormat("{0}: Preparing {1} objects of {2} ({3}). Is count valid?", GetType().Name, count,
                    prefab.name, prefab.GetType().Name);
            }

            if (!forceAddNew)
            {
                count -= GetTotalCount(prefab);

                if (count <= 0)
                {
                    return;
                }
            }

            if (!_freeObjects.TryGetValue(prefab, out Queue<QueueEntry> freeQueue))
            {
                freeQueue = new Queue<QueueEntry>(count);
                _freeObjects.Add(prefab, freeQueue);
            }

            parent = parent ?? DefaultParent;

            for (int i = 0; i < count; i++)
            {
                var obj = Object.Instantiate(prefab, parent);
                obj.SetPool(this);

                // NOTE: некоторые объекты делают дополнительную инициализацию
                obj.Prepare(data);

                obj.gameObject.SetActive(false);

                freeQueue.Enqueue(new QueueEntry(obj, true));
            }

            EntryInstantiated(prefab, count);
        }

        public int GetTotalCount(PooledBehaviour prefab = null)
        {
            if (prefab != null)
            {
                return _busyObjects.Count(p => p.Value == prefab) +
                       _freeObjects.Where(f => f.Key == prefab).Sum(f => f.Value.Count);
            }
            else
            {
                return _busyObjects.Count + _freeObjects.Sum(f => f.Value.Count);
            }
        }

        public int GetFreeCount(PooledBehaviour prefab)
        {
            if (_freeObjects.TryGetValue(prefab, out Queue<QueueEntry> freeQueue))
            {
                return freeQueue.Count;
            }
            else
            {
                return 0;
            }
        }

        public Dictionary<PooledBehaviour, int> GetFreeCounts()
        {
            var result = new Dictionary<PooledBehaviour, int>();
            foreach (var pair in _freeObjects)
            {
                result.Add(pair.Key, pair.Value.Count);
            }

            return result;
        }

        public Dictionary<PooledBehaviour, int> GetBusyCounts()
        {
            var result = new Dictionary<PooledBehaviour, int>();
            foreach (var pair in _busyObjects)
            {
                if (result.TryGetValue(pair.Value, out int count))
                {
                    count++;
                }
                else
                {
                    count = 1;
                }

                result[pair.Value] = count;
            }

            return result;
        }

        public void DestroyAll(PooledBehaviour prefab)
        {
            FreeAll(prefab);

            if (_freeObjects.TryGetValue(prefab, out Queue<QueueEntry> queue))
            {
                foreach (var entry in queue)
                {
                    Object.Destroy(entry.Object.gameObject);
                }

                _freeObjects.Remove(prefab);
            }
        }

        public void ApplyToAll(PooledBehaviour prefab, Action<PooledBehaviour> action)
        {
            if (_freeObjects.TryGetValue(prefab, out Queue<QueueEntry> freeQueue))
            {
                foreach (var entry in freeQueue)
                {
                    action(entry.Object);
                }
            }

            foreach (var pair in _busyObjects)
            {
                if (pair.Value == prefab)
                {
                    action(pair.Key);
                }
            }
        }

        private void Free(PooledBehaviour obj, PooledBehaviour prefab, bool resetParent)
        {
            if (!_freeObjects.TryGetValue(prefab, out Queue<QueueEntry> freeQueue))
            {
                freeQueue = new Queue<QueueEntry>(_capacity);
                _freeObjects[prefab] = freeQueue;
            }

            freeQueue.Enqueue(new QueueEntry(obj, false));
            // NOTE: Некоторым объектам может быть нужна обработка, пока их не деактивировали.
            // дестрой обектам был вызван из quit match
            if (obj != null)
            {
                if (!obj.BeforeReturnToPool())
                {
                    return;
                }

                obj.OnReturnToPool();
                obj.ClearData();

                obj.gameObject.SetActive(false);

                if (resetParent && obj.transform.parent != DefaultParent)
                {
                    obj.transform.SetParent(DefaultParent, false);
                }
            }
        }
        
        public PooledBehaviour GetObject(PoolObjectType type, Vector3 pos, Quaternion rot)
        {
            return Get(GetPooledBehaviourPrefab(type), position: pos, rotation: rot);
        }

        public PooledBehaviour GetPooledBehaviourPrefab(PoolObjectType type)
        {
            return _coreConfig.PoolObjects.PooledPrefabs.GetValue(type).prefab;
        }
    }
}