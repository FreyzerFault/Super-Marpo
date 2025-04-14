using UnityEngine;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    
        public void Restart() => GameManager.ReloadScene();
        public void Exit() => GameManager.LoadScene("MainMenu");
    }
}
