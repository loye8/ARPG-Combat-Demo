using UnityEngine;

namespace ARPGCombat.MVC.View
{
    public class ViewEvents : MonoBehaviour
    {
        public const string PlayerCreated = "PlayerCreated";
        public const string CharacterDamaged = "CharacterDamaged";
        public const string CharacterHeal = "CharacterHeal";
        public const string CharacterDied = "CharacterDied";
        public const string GameStateChanged = "GameStateChanged";
        public const string EnemyCreated = "EnemyCreated";
        public const string PlayerSkillCast = "PlayerSkillCast";
    }
}
