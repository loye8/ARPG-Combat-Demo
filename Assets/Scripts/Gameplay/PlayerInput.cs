using UnityEngine;

namespace ARPGCombat.Gameplay {
    [DefaultExecutionOrder(-100)]
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MoveAxis { get; private set; }
        public bool AttackPressed { get; private set; }

        public bool SkillQPressed { get; private set; }

        public bool SkillEPressed { get; private set; }

        public bool RollPressed { get; private set; }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            MoveAxis = new Vector2(h, v).normalized;

            AttackPressed = Input.GetMouseButton(0);
            SkillQPressed = Input.GetKeyDown(KeyCode.Q);
            SkillEPressed = Input.GetKeyDown(KeyCode.E);
            RollPressed = Input.GetKeyDown(KeyCode.Space);
        }
    }
}
