using System.Collections;
using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [SerializeField] private int sleepDuration = 8; // Uyuma süresi saat cinsinden
    [SerializeField] private GameObject fadePanel; // Siyah ekran paneli
    [SerializeField] private GameObject ePopUp; 
    [SerializeField] PlayerController playerController;

    private bool isPlayerNearby;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SleepRoutine());
        }
    }

    private IEnumerator SleepRoutine()
    {
        // Zamanı durdur
        Time.timeScale = 0f;
        
        // Ekranı karart
        fadePanel.SetActive(true);
        playerController.SetMovementEnabled(false); // Hareketi kontrol et
        yield return new WaitForSecondsRealtime(1f); // Ekran kararma süresi

        // Zamanı atlat
        GameTimeManager.Instance.AddHours(sleepDuration);

        // Ekranı geri aç
        yield return new WaitForSecondsRealtime(1f);
        fadePanel.SetActive(false);
        playerController.SetMovementEnabled(true); // Hareketi kontrol et
        
        // Zamanı tekrar başlat
        Time.timeScale = 1f;
    }
    
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player")
        {
            isPlayerNearby = true;
            ePopUp.SetActive(true);
            Debug.Log("Yatağa yaklaşıldı. 'E' tuşuna basarak uyuyabilirsiniz.");
        }
    }
    
    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Player")
        {
            ePopUp.SetActive(false);
            isPlayerNearby = false;
        }
    }
}
