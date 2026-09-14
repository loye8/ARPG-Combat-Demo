using UnityEngine;
using ARPGCombat.Core;
using ARPGCombat.MVC.Model;

namespace ARPGCombat.Gameplay
{
    public class DamageSystem : MonoBehaviour
    {
        [Header("伤害公式参数")]
        [Tooltip("基础暴击率(角色自身暴击率为0时的保底)")]
        [Range(0f, 1f)]
        public float baseCriticalChance = 0.15f;

        [Tooltip("基础暴击倍率，1.5 = 150% 伤害")]
        [Range(1f, 3f)]
        public float criticalMultiplier = 1.5f;

        [Tooltip("伤害浮动范围，0.1 = +-10%")]
        [Range(0f, 0.3f)]
        public float damageVariance = 0.1f;

        private void OnEnable()
        {
            //玩家命中
            EventCenter.Instance.On("PlayerAttackHit", HandlePlayerAttackHit);
            //敌人命中
            EventCenter.Instance.On("EnemyAttackHit", HandleEnemyAttackHit);
        }

        private void OnDisable()
        {
            EventCenter.Instance?.Off("PlayerAttackHit", HandlePlayerAttackHit);
            EventCenter.Instance?.Off("EnemyAttackHit", HandleEnemyAttackHit);
        }

        private void HandlePlayerAttackHit(object data)
        {
            if (data == null) return;
            System.Type t = data.GetType();
            int attackerId = ReadInt(t, data, "instigatorId");
            Collider targetCollider = ReadObject(t, data, "targetCollider") as Collider;
            int rawDamage = ReadInt(t, data, "rawDamage");
            Vector3 hitPoint = ReadPoint(t, data, "hitPoint", targetCollider);

            var targetModel = CharacterRegistry.Instance.GetByCollider(targetCollider);
            if (targetModel == null)
            {
                Debug.Log($"[DamageSystem]命中对象{targetCollider?.name}未注册，已忽略。");
                return;
            }

            ProcessDamage(attackerId, attackerPower: rawDamage, targetModel, hitPoint);
        }

        private void HandleEnemyAttackHit(object data)
        {
            if (data == null) return;
            System.Type t = data.GetType();
            int attackerId = ReadInt(t, data, "instigatorId");
            Collider targetCollider = ReadObject(t, data, "targetCollider") as Collider;
            int rawDamage = ReadInt(t, data, "rawDamage");
            Vector3 hitPoint = ReadPoint(t, data, "hitPoint", targetCollider);

            var targetModel = CharacterRegistry.Instance.GetByCollider(targetCollider);
            if (targetModel == null) return;

            ProcessDamage(attackerId, rawDamage, targetModel, hitPoint);
        }

        private void ProcessDamage(int attackerId,int attackerPower,CharacterModel targetModel,Vector3 hitPoint)
        {
            if (targetModel.IsDead) return;
            bool isCrit = Random.value < baseCriticalChance;
            float damage = attackerPower;
            if (isCrit) damage *= criticalMultiplier;

            float variance = 1f - damageVariance + Random.value * damageVariance * 2f;
            damage *= variance;

            int finalRaw = Mathf.Max(1, Mathf.RoundToInt(damage));

            targetModel.TakeDamage(attackerId, finalRaw, hitPoint, isCrit);
        }

        // Unity 默认程序集不支持 dynamic，改用反射读取匿名载荷（hitPoint 缺失时用碰撞体位置兜底）
        private static object ReadObject(System.Type t, object data, string name)
        {
            var p = t.GetProperty(name);
            return p != null ? p.GetValue(data) : null;
        }

        private static int ReadInt(System.Type t, object data, string name)
        {
            var v = ReadObject(t, data, name);
            return v is int i ? i : 0;
        }

        private static Vector3 ReadPoint(System.Type t, object data, string name, Collider fallback)
        {
            var v = ReadObject(t, data, name);
            if (v is Vector3 vec) return vec;
            return fallback != null ? fallback.transform.position : Vector3.zero;
        }
    }
}
