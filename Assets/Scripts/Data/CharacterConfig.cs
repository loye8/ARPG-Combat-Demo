using System.IO.Enumeration;
using UnityEngine;

namespace ARPGCombat.Data
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "ARPGCombat/Character Config", order = 0)]

    public class CharacterConfig : ScriptableObject
    {
        [Header("基础属性")]
        [Tooltip("最大血量")]
        public int maxHp = 100;

        [Tooltip("攻击力")]
        public int attackPower = 20;

        [Tooltip("防御率（0-1),0.1 表示减伤 10%")]
        [Range(0f, 1f)]
        public float defenseRate = 0.1f;

        [Header("移动属性")]
        [Tooltip("移动速度（米/秒）")]
        public float moveSpeed = 6f;

        [Tooltip("转身速度")]
        public float rotationSpeed = 15f;

        [Header("攻击属性")]
        [Tooltip("攻击范围(OverlapSphere 半径,米)")]
        public float attackRange = 2.5f;

        [Tooltip("攻击冷却时间(秒)")]
        public float attackCooldown = 0.5f;

        [Tooltip("攻击前摇(秒),挥剑前的抬手时间")]
        public float attackWindup = 0.15f;

        [Header("身份标识")]
        [Tooltip("角色标签: Player/Enemy,用于击杀计数判断")]
        public string characterTag = "Player";
    }
}
