using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARPGCombat.Core
{
    public class EventCenter : Singleton<EventCenter>
    {
        private readonly Dictionary<string, Action<object>> _events = new();
        public void On(string eventName, Action<object> callback)
        {
            if (callback == null) return;

            if (_events.TryGetValue(eventName, out var existing))
            {
                _events[eventName] = existing + callback;
            }
            else
            {
                _events.Add(eventName,callback);
            }
        }

        public void Off(string eventName, Action<object> callback)
        {
            if(callback == null) return;
            if (!_events.TryGetValue(eventName, out var existing)) return;

            var newDelegate = existing - callback;

            if (newDelegate == null)
            {
                _events.Remove(eventName);
            }
            else
            {
                _events[eventName] = newDelegate;
            }
        }

        public void Emit(string eventName, object data = null)
        {
            if (_events.TryGetValue(eventName, out var existing))
            {
                existing?.Invoke(data);
            }
        }

        public void OffAll(string eventName)
        {
            _events.Remove(eventName);
        }

        public void Clear()
        { 
            _events.Clear();
        }

        public int GetListenerCount(string eventName)
        {
            if (!_events.TryGetValue(eventName, out var existing)) return 0;

            return existing?.GetInvocationList().Length?? 0;
        }

        protected override void OnDestory()
        {
            _events.Clear();
            base.OnDestory();
        }
    }   
}