using UnityEngine;

public class MainMenu : MonoBehaviour
{
    
    public void StartGame() => GameManager.LoadScene("Game");
    public void Exit() => GameManager.QuitGame();
}
