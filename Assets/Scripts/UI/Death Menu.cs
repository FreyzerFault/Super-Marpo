using Managers;
using Player;
using UnityEngine;

namespace UI
{
    public class DeathMenu : PopUpMenu
    {
        [SerializeField] private float delaySeconds = 1f;
        
        private void Start() => PlayerController.Instance.OnDeath += HandlePlayerDeath;
        private void OnDestroy() => PlayerController.Instance.OnDeath -= HandlePlayerDeath;

        private void HandlePlayerDeath() => ShowWithDelay(delaySeconds);
        

        public void Restart() => GameManager.ReloadScene();
        public void Exit() => GameManager.LoadScene("MainMenu");
    }
}
