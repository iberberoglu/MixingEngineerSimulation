using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MixTableInteraction : MonoBehaviour
{
    [SerializeField] private GameObject mixTable; // Mix Table objesi
    [SerializeField] private GameObject menuCanvas; // Menü için Canvas objesi
    private bool isNearMixTable = false; // Mix Table'a yakın mı?
    private bool isMixMenuActive = false; // Menü açık mı?

    private PlayerController playerController; // Player hareketini kontrol etmek için
    [SerializeField] private PlayerCameraMovement playerCameraMovement;
    [SerializeField] private GameObject pressEPopup;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == mixTable)
        {
            isNearMixTable = true;
        }
        InteractPopup();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == mixTable)
        {
            isNearMixTable = false;
        }
        InteractPopup();
    }


    public void OnInteract(InputValue value)
    {
        if (isNearMixTable)
        {
            isMixMenuActive = !isMixMenuActive; // Menü açık/kapalı durumunu değiştir
            menuCanvas.SetActive(isMixMenuActive); // Canvas'ı aç/kapat
            if(pressEPopup.activeSelf && isMixMenuActive)
            {
                pressEPopup.SetActive(false);
            }
            else
            {
                pressEPopup.SetActive(true);    
            }
            
            if (playerController != null)
            {
                playerController.SetMovementEnabled(!isMixMenuActive); // Hareketi kontrol et
            }

            if (playerCameraMovement != null)
            {
                playerCameraMovement.OnCanvasStateChanged(isMixMenuActive); // Kamera durumu değişimi
            }
        }
    }
    
    private void InteractPopup()
    {
        if (isNearMixTable)
        {
            pressEPopup.SetActive(true);
        }
        else
        {
            pressEPopup.SetActive(false);
        }
    }
}
