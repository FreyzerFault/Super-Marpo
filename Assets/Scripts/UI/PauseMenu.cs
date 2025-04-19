using Managers;

namespace UI
{
    public class PauseMenu : PopUpMenu
    {
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
    
        public void Restart() => GameManager.ReloadScene();
        public void Exit() => GameManager.LoadScene("MainMenu");
    }
}
