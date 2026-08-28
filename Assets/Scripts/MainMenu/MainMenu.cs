using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene"); // Oyun sahnesine geçiş yapar
    }

    public void QuitGame()
    {
        Debug.Log("Game is quitting...");
        Application.Quit(); // Oyunu kapatır
    }
}
