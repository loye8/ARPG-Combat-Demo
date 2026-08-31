using UnityEngine;

namespace ARPGCombat.Data
{
    [CreateAssetMenu(fileName = "SkillConfig", menuName = "ARPGCombat/Skill Config", order = 1)]
    public class SkillConfig : ScriptableObject
    {
        [Header("基本信息")]
        [Tooltip("技能名,UI 显示用")]
        public string skillName = "Skill";

        [Tooltip("技能描述,UI 显示用")]
        [TextArea(2, 4)]
        public string description = "";

        [Tooltip("技能图标,UI 显示用")]
        public Sprite icon;

        [Tooltip("技能 ID: 0=Q 键, 1=E 键")]
        [Range(0, 1)]
        public int skillId = 0;

        [Header("伤害")]
        [Tooltip("基础伤害值")]
        public int baseDamage = 30;

        [Tooltip("暴击率(0-1)")]
        [Range(0f, 1f)]
        public float criticalChance = 0.2f;

        [Tooltip("暴击伤害倍率(1.5 = 暴击造成 150% 伤害)")]
        [Range(1f, 3f)]
        public float criticalMultiplier = 1.5f;

        [Header("范围")]
        [Tooltip("作用半径(米),AOE 技能用")]
        public float effectRadius = 3f;

        [Tooltip("作用角度(度),扇形技能用。360=全圆 AOE")]
        [Range(15f, 360f)]
        public float effectAngle = 360f;

        [Header("冷却与消耗")]
        [Tooltip("冷却时间(秒)")]
        public float cooldown = 5f;

        [Tooltip("资源消耗(法力/怒气等)")]
        public int resourceCost = 20;

        [Header("表现")]
        [Tooltip("特效预制体路径(Resources.Load 用),如 'VFX/SkillEffect_Q'")]
        public string effectPrefabPath = "";

        [Tooltip("动画参数名(Animator trigger),如 'SkillQ'")]
        public string animTriggerName = "SkillQ";

        [Tooltip("技能前摇时间(秒)")]
        public float windup = 0.3f;
    }
}
