using System.Collections;
using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [SerializeField] private int sleepDuration = 8; // Uyuma süresi saat cinsinden
    [SerializeField] private GameObject fadePanel; // Siyah ekran paneli
    [SerializeField] private GameObject ePopUp; 
    [SerializeField] PlayerController playerController;

    private bool isPlayerNearby;
    private bool isSleeping = false;

    private void Update()
    {
        if (isPlayerNearby && !isSleeping && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SleepRoutine());
        }
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true; // Uyuma işlemini başlat
        playerController.SetMovementEnabled(false); // Hareketi kontrol et
        // Zamanı durdur
        Time.timeScale = 0f;
        
        // Ekranı karart
        fadePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(1f); // Ekran kararma süresi

        // Zamanı atlat
        GameTimeManager.Instance.AddHours(sleepDuration);

        // Ekranı geri aç
        yield return new WaitForSecondsRealtime(1f);
        fadePanel.SetActive(false);
        
        // Zamanı tekrar başlat
        Time.timeScale = 1f;
        playerController.SetMovementEnabled(true); // Hareketi kontrol et
        isSleeping = false; // Uyuma işlemi tamamlandı
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
