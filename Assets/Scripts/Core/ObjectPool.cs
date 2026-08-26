using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace ARPGCombat.Core
{
    public class ObjectPool<T> where T : class
    {
        private readonly Queue<T> _pool;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;
        private readonly int _maxSize;
        public int CountAll { get; private set; }
        public int CountInactive => _pool.Count;

        public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onReturn = null, int maxSize = 100)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onReturn = onReturn;
            _maxSize = Mathf.Max(1, maxSize);
            _pool = new Queue<T>(_maxSize);
        }

        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else 
            {
                obj = _createFunc();
                CountAll++;
            }

            _onGet?.Invoke(obj);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null) return;
            _onReturn?.Invoke(obj);

            if (_pool.Count < _maxSize)
            {
                _pool.Enqueue(obj);
            }
            else
            {
                if (obj is UnityEngine.Object uo) UnityEngine.Object.Destroy(uo);
                CountAll--;
            }
        }

        public void Prewarm(int count)
        {
            for(int i = 0; i < count; i++)
            {
                var obj =_createFunc();
                CountAll++;
                _onReturn?.Invoke(obj);
                _pool.Enqueue(obj);
            }
        }

        public void Clear(bool destroyObjects = false)
        {
            if (destroyObjects)
            {
                while(_pool.Count > 0)
                {
                    var obj = _pool.Dequeue();
                    if (obj is UnityEngine.Object uo) UnityEngine.Object.Destroy(uo);
                }
                CountAll = 0;
            }
            else
            {
                _pool.Clear();
            }
        }
    }
}
