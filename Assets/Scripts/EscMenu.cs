using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscMenu : MonoBehaviour
{
    [SerializeField] private GameObject escMenuPanel; // ESC menüsü Canvas Panel

    private bool isMenuOpen = false;

    private void Start()
    {
        if (escMenuPanel != null)
        {
            escMenuPanel.SetActive(false); // Başlangıçta menüyü gizle
        }
    }

    public void OnToggleMenu()
    {
            isMenuOpen = !isMenuOpen;
            escMenuPanel.SetActive(isMenuOpen); // Menüyü aç/kapat
    }

    public void ExitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
        SceneManager.LoadScene("MainMenu"); // Ana menü sahnesine geçiş yapar
    }
}
