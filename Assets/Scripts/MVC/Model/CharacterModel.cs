using System;
using UnityEngine;
using ARPGCombat.Core;
using ARPGCombat.Data;

namespace ARPGCombat.MVC.Model
{
    public struct DamageEventData
    {
        /// <summary>攻击者实例 ID</summary>
        public int instigatorId;
        /// <summary>被击者实例 ID</summary>
        public int targetId;
        public int damage;
        public bool isCritical;
        public bool isDead;
        public Vector3 hitPoint;
        public string targetTag;
    }
    public class CharacterModel
    {
        //实例 ID,用于 CharacterRegistry 查表
        public int InstanceId { get; }
        //角色标签(Player/Enemy),BattleController 判断击杀计数用
        public string Tag { get; }
        public int MaxHp { get; }
        public int AttackPower { get; }
        public float MoveSpeed { get; }
        public float RotationSpeed { get; }
        public float AttackRange { get; }
        public float AttackCooldown { get; }
        public float AttackWindup { get; }
        public float DefenseRate { get; }
        public int CurrentHp { get; private set; }
        public bool IsDead { get; private set; }
        private float _lastAttackTime;
        public CharacterModel(int instanceId, CharacterConfig config)
        {
            InstanceId = instanceId;
            Tag = config.characterTag;
            MaxHp = config.maxHp;
            AttackPower = config.attackPower;
            MoveSpeed = config.moveSpeed;
            RotationSpeed = config.rotationSpeed;
            AttackRange = config.attackRange;
            AttackCooldown = config.attackCooldown;
            AttackWindup = config.attackWindup;
            DefenseRate = config.defenseRate;

            CurrentHp = MaxHp;
            IsDead = false;
            _lastAttackTime = -999f;
        }

        public void TakeDamage(int instigatorId,int rawDamage,Vector3 hitPoint,bool isCritical)
        {
            if(IsDead) return;

            int finalDamage = Mathf.Max(1, Mathf.RoundToInt(rawDamage * (1f - DefenseRate)));

            CurrentHp -= finalDamage;
            if(CurrentHp < 0) CurrentHp = 0;

            bool willDie = CurrentHp <= 0;
            if (willDie) IsDead = true;

            var eventData = new DamageEventData
            {
                instigatorId = instigatorId,
                targetId = InstanceId,
                damage = finalDamage,
                isCritical = isCritical,
                isDead = willDie,
                hitPoint = hitPoint,
                targetTag = Tag
            };

            EventCenter.Instance?.Emit("CharacterDamaged", eventData);

            if (willDie)
            {
                EventCenter.Instance?.Emit("CharacterDied", eventData);
            }
        }

        public void Heal(int amount,Vector3 healPoint)
        {
            if(IsDead) return;
            if(amount <= 0) return;

            int oldHp = CurrentHp;
            CurrentHp = Mathf.Min(CurrentHp + amount, MaxHp);
            int actualHeal = CurrentHp - oldHp;

            if(actualHeal > 0)
            {
                var eventData = new DamageEventData
                {
                    instigatorId = -1,
                    targetId = InstanceId,
                    damage = -actualHeal,
                    isCritical = false,
                    isDead = false,
                    hitPoint = healPoint,
                    targetTag = Tag
                };
                EventCenter.Instance?.Emit("CharacterHeal", eventData);
            }

        }

        public bool CanAttack(float currentTime)
        {
            if(IsDead) return false;
            return currentTime - _lastAttackTime >= AttackCooldown;
        }

        public void MarkAttacked(float currentTime)
        {
            _lastAttackTime = currentTime;
        }

        public float GetHpRatio()
        {
            if(MaxHp <= 0) return 0f;
            return (float)CurrentHp / MaxHp;
        }
    }
}
