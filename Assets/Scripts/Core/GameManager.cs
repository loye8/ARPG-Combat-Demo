using System;
using UnityEngine;
using UnityEngine.XR;

namespace ARPGCombat.Core
{
    public enum GameState
    {
        Boot,       
        Playing,    
        Paused,     
        GameOver    
    }

    public class GameManager : Singleton<GameManager>
    {
        public GameState CurrentState { get; private set; } = GameState.Boot;

        public event Action<GameState> OnStateChanged;

        protected override void Awake()
        {
            base.Awake();

            if(CurrentState == GameState.Boot)
            {
                InitFramework();
            }
        }

        private void InitFramework()
        {
            _ = EventCenter.Instance;
            Debug.Log("[GameManager] ¿ò¼Ü³õÊ¼»¯Íê³É");
        }

        private void Start()
        {
            ChangeState(GameState.Playing);
        }

        public void ChangeState(GameState newState)
        {
            if (newState == CurrentState) return;

            var oldState = CurrentState;
            CurrentState = newState;

            Debug.Log($"[GameManager] ×´Ì¬ÇÐ»»:{oldState} ¡ú {newState}");

            OnStateChanged?.Invoke(newState);

            EventCenter.Instance?.Emit("GameStateChanged", newState);
        }

        public void Pause()
        {
            if(CurrentState == GameState.Playing)
            {
                Time.timeScale = 0;
                ChangeState(GameState.Paused); 
            }
        }

        public void Resume()
        {
            if(CurrentState == GameState.Paused)
            {
                Time.timeScale = 1;
                ChangeState(GameState.Playing);
            }
        }

        public void GameOver()
        {
            ChangeState(GameState.GameOver);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
