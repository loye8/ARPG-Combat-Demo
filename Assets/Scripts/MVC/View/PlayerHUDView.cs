using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARPGCombat.MVC.Model;

namespace ARPGCombat.MVC.View
{
    public class PlayerHUDView : MonoBehaviour
    {
        [SerializeField] private Image hpFill;
        [SerializeField] private TextMeshProUGUI hpText;
        private CharacterModel _player;

        public void BindPlayer(CharacterModel model)
        {
            _player = model;
            RefreshHp();
        }
        public void RefreshHp()
        {
            if (_player == null) return;
            hpFill.fillAmount = _player.GetHpRatio();
            hpText.text = $"{_player.CurrentHp}/{_player.MaxHp}";
        }
    }
}
 