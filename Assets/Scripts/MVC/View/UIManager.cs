using UnityEngine;
using ARPGCombat.Core;
using ARPGCombat.MVC.Model;
using System;
namespace ARPGCombat.MVC.View
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PlayerHUDView hudView;
        [SerializeField] private EnemyHUDView enemyHUDView;
        [SerializeField] private DamageNumberLayer damageLayer;
        [SerializeField] private PanelViewController panels;
        [SerializeField] private SkillBarView skillBar;
        private void OnEnable()
        {
            var ec = EventCenter.Instance;
            ec.On(ViewEvents.PlayerCreated, OnPlayerCreated);
            ec.On(ViewEvents.CharacterDamaged, OnCharacterDamaged);
            ec.On(ViewEvents.CharacterHeal, OnCharacterHeal);
            ec.On(ViewEvents.CharacterDied, OnCharacterDied);
            ec.On(ViewEvents.GameStateChanged, OnGameStateChanged);
            ec.On(ViewEvents.EnemyCreated, OnEnemyCreated);
            ec.On(ViewEvents.PlayerSkillCast, OnPlayerSkillCast);
        }

        private void OnDisable()
        {
            var ec = EventCenter.Instance;
            if (ec == null) return;
            ec.Off(ViewEvents.PlayerCreated, OnPlayerCreated);
            ec.Off(ViewEvents.CharacterDamaged, OnCharacterDamaged);
            ec.Off(ViewEvents.CharacterHeal, OnCharacterHeal);
            ec.Off(ViewEvents.CharacterDied, OnCharacterDied);
            ec.Off(ViewEvents.GameStateChanged, OnGameStateChanged);
            ec.Off(ViewEvents.EnemyCreated, OnEnemyCreated);
            ec.Off(ViewEvents.PlayerSkillCast, OnPlayerSkillCast);
        }

        private void OnPlayerCreated(object data)
        {
            if (hudView != null)
                hudView.BindPlayer(ReadProp(data, "model") as CharacterModel);
        }
        private void OnCharacterDamaged(object data)
        {
            var d = (DamageEventData)data;
            if (d.targetTag == "Player")
                hudView?.RefreshHp();
            damageLayer?.Spawn(d.hitPoint,d.damage.ToString(),d.isCritical);
        }
        private void OnCharacterHeal(object data)
        {
            var d = (DamageEventData)data;
            if (d.targetTag == "Player") hudView?.RefreshHp();
            damageLayer?.Spawn(d.hitPoint, "+" + (-d.damage), false, Color.green);
        }
        private void OnCharacterDied(object data)
        {
            var d = (DamageEventData)data;
            if (d.targetTag == "Player")
                return;
            enemyHUDView?.RemoveBar(d.targetId);            
        }
        private void OnGameStateChanged(object data) => panels?.SetState((GameState)data);
        private void OnEnemyCreated(object data)
        {
            if (enemyHUDView != null)
                enemyHUDView.AddBar(
                    (int)ReadProp(data, "instanceId"),
                    ReadProp(data, "transform") as Transform,
                    ReadProp(data, "model") as CharacterModel);
        }
        private void OnPlayerSkillCast(object data) => skillBar?.OnSkillCast(data);

        private static object ReadProp(object obj, string propName)
            => obj?.GetType().GetProperty(propName)?.GetValue(obj);
    }
}
