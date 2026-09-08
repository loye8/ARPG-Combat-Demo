using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARPGCombat.MVC.View
{
    public class SkillBarView : MonoBehaviour
    {
        private class Slot
        {
            public int skillId;
            public Image overlay;          
            public TextMeshProUGUI cdText; 
            public float cooldown;
            public float remaining;
            public bool cooling;
        }

        private readonly List<Slot> _slots = new();

        private void Awake()
        {
            BindSlots();
        }

        private void BindSlots()
        {
            _slots.Clear();
            foreach (Transform child in transform)
            {
                int id = DetectSkillId(child.name);
                if (id < 0) continue;

                var slot = new Slot { skillId = id };

                foreach (var img in child.GetComponentsInChildren<Image>(true))
                {
                    if (img.type == Image.Type.Filled)
                    {
                        slot.overlay = img;
                        break;
                    }
                }
  
                slot.cdText = child.GetComponentInChildren<TextMeshProUGUI>(true);

                if (slot.overlay != null)
                {
                    slot.overlay.fillAmount = 0f;   // 空闲时全亮，不显示遮罩
                    slot.overlay.raycastTarget = false;
                }
                if (slot.cdText != null) slot.cdText.text = "";

                _slots.Add(slot);
                string overlayName = slot.overlay != null ? slot.overlay.name : "无";
                string textName = slot.cdText != null ? slot.cdText.name : "无";
                Debug.Log($"[SkillBar] 绑定技能槽 {child.name} -> skillId={id}, 遮罩={overlayName}, 文字={textName}");
            }
        }
        public void OnSkillCast(object data)
        {
            int skillId = ReadInt(data, "skillId");
            float cd = ReadFloat(data, "cooldown");

            foreach (var s in _slots)
            {
                if (s.skillId != skillId) continue;
                s.cooldown = Mathf.Max(0.01f, cd);
                s.remaining = cd;
                s.cooling = true;
                break;
            }
        }

        private void Update()
        {
            foreach (var s in _slots)
            {
                if (!s.cooling) continue;

                s.remaining -= Time.deltaTime;   // 缩放时间：暂停时冷却同步冻结
                if (s.remaining <= 0f)
                {
                    s.remaining = 0f;
                    s.cooling = false;
                }

                // 同一个计时器同步驱动遮罩和倒计时
                if (s.overlay != null)
                    s.overlay.fillAmount = s.cooldown > 0f ? s.remaining / s.cooldown : 0f;

                if (s.cdText != null)
                    s.cdText.text = s.cooling ? s.remaining.ToString("F1") : "";
            }
        }

        private static int DetectSkillId(string name)
        {
            if (name.IndexOf("Q", StringComparison.OrdinalIgnoreCase) >= 0) return 0;
            if (name.IndexOf("E", StringComparison.OrdinalIgnoreCase) >= 0) return 1;
            return -1;
        }

        // Unity 默认程序集不支持 dynamic，用反射读取匿名对象属性
        private static int ReadInt(object obj, string name)
            => obj?.GetType().GetProperty(name)?.GetValue(obj) is int v ? v : 0;

        private static float ReadFloat(object obj, string name)
            => obj?.GetType().GetProperty(name)?.GetValue(obj) is float v ? v : 0f;
    }
}
