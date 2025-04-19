using Managers;
using UnityEngine;

namespace UI
{
    public class EndUI : MonoBehaviour
    {
        public void QuitToMenu() => GameManager.LoadScene("MainMenu");
    }
}
