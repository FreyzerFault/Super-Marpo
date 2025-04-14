using System;
using UnityEngine;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        private GameObject _menu;

        private void Awake()
        {
            _menu = transform.GetChild(0).gameObject;
            Hide();
        }

        private void Start() => GameManager.Instance.onGameStateChanged += OnGameStateChanged;
        private void OnDestroy() => GameManager.Instance.onGameStateChanged -= OnGameStateChanged;

        private void OnGameStateChanged()
        {
            switch (GameManager.Instance.State)
            {
                case GameManager.GameState.MainMenu:
                case GameManager.GameState.GameOver:
                    break;
                case GameManager.GameState.Playing:
                    Hide();
                    break;
                case GameManager.GameState.Paused:
                    Show();
                    break;
            }
        }

        public void Toggle() => _menu.SetActive(!gameObject.activeSelf);
        public void Show() => _menu.SetActive(true);
        public void Hide() => _menu.SetActive(false);
    
        public void Restart() => GameManager.ReloadScene();
        public void Exit() => GameManager.LoadScene("MainMenu");
    }
}
