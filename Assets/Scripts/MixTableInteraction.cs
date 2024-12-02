using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MixTableInteraction : MonoBehaviour
{
    [SerializeField] private GameObject mixTable; // Mix Table objesi
    [SerializeField] private GameObject menuCanvas; // Menü için Canvas objesi
    private bool isNearMixTable = false; // Mix Table'a yakın mı?
    private bool isMenuActive = false; // Menü açık mı?

    private PlayerController playerController; // Player hareketini kontrol etmek için
    [SerializeField] private PlayerCameraMovement playerCameraMovement;

    private void Start()
    {
        // PlayerController script'ini bul
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == mixTable)
        {
            isNearMixTable = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == mixTable)
        {
            isNearMixTable = false;
        }
    }


public void OnInteract(InputValue value)
{
    if (isNearMixTable)
    {
        isMenuActive = !isMenuActive; // Menü açık/kapalı durumunu değiştir
        menuCanvas.SetActive(isMenuActive); // Canvas'ı aç/kapat

        if (playerController != null)
        {
            playerController.SetMovementEnabled(!isMenuActive); // Hareketi kontrol et
        }

        if (playerCameraMovement != null)
        {
            playerCameraMovement.OnCanvasStateChanged(isMenuActive); // Kamera durumu değişimi
        }
    }
}
}
