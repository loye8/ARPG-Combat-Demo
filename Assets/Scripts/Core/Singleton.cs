using UnityEngine;

namespace ARPGCombat.Core
{

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get {
                if (_applicationIsQuitting)
                    return null;

                lock (_lock)
                { 
                if(_instance != null)
                        return _instance;

                    _instance = FindFirstObjectByType<T>();
                    if(_instance != null)
                        return _instance;
                }
                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }

            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] 检测到 {typeof(T).Name} 的重复实例,已销毁。");
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;  
        }

        protected virtual void OnDestory()
        {
            if(_instance == this)
                _instance = null;
        }
    }
}