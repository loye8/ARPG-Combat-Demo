using ARPGCombat.Core;
using UnityEngine;

public class PanelViewController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    public void SetState(GameState state)
    {
        pausePanel.SetActive(state == GameState.Paused);
        gameOverPanel.SetActive(state == GameState.GameOver);
    }
}
