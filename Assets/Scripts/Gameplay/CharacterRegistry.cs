using System.Collections.Generic;
using UnityEngine;
using ARPGCombat.Core;
using ARPGCombat.MVC.Model;

namespace ARPGCombat.Gameplay
{
    public class CharacterRegistry : Singleton<CharacterRegistry>
    {
        private readonly Dictionary<int, CharacterModel> _byId = new();
        private readonly Dictionary<Collider, CharacterModel> _byCollider = new();

        public void Register(Collider collider, CharacterModel model)
        {
            if (collider == null || model == null) return;
            if (_byId.ContainsKey(model.InstanceId))
            {
                Debug.LogWarning($"[CharacterRegistry] ID {model.InstanceId} ÖØ¸´×¢²á,ÒÑºöÂÔ¡£");
                return;
            }

            if (_byCollider.ContainsKey(collider))
            {
                Debug.LogWarning($"[CharacterRegistry] Collider {collider.name} ÖØ¸´×¢²á,ÒÑºöÂÔ¡£");
                return;
            }

            _byId.Add(model.InstanceId, model);
            _byCollider.Add(collider, model);
        }

        public void Unregister(int instanceId, Collider collider)
        {
            if (_byId.ContainsKey(instanceId)) _byId.Remove(instanceId);
            if (collider != null && _byCollider.ContainsKey(collider)) _byCollider.Remove(collider);
        }

        public CharacterModel GetById(int id)
        {
            _byId.TryGetValue(id, out var model);
            return model;
        }

        public int Count => _byId.Count;

        public CharacterModel GetByCollider(Collider c)
        {
            if (c == null) return null;
            _byCollider.TryGetValue(c, out var model);
            return model;
        }
    }
}
