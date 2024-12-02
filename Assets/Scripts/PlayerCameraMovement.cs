using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Ana kamera
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera; // Cinemachine kameranız
    [SerializeField] private Vector3 offset; // Kamera hareketinin hedef noktası
    private bool isCanvasActive = false; // Canvas durumu
    [SerializeField] float cameraSpeed = 0.1f;
    private float cameraTargetPositon;

    public void OnCanvasStateChanged(bool isActive)
    {
        isCanvasActive = isActive;

        if (isCanvasActive)
        {
            // Cinemachine'i devre dışı bırak ve kamera hareketini başlat
            cinemachineCamera.enabled = false;
            cameraTargetPositon = mainCamera.transform.position.y + offset.y;
            StartCoroutine(MoveCamera(cameraSpeed));
        }
        else
        {
            // Kamera tekrar Player'ı takip etsin
            cinemachineCamera.enabled = true;
            StopCoroutine(MoveCamera(cameraSpeed));
        }
    }
    
    IEnumerator MoveCamera(float cameraSpeed)
    {
    while (Mathf.Abs(cameraTargetPositon - mainCamera.transform.position.y) > 0.01f)
    {
        // Yeni pozisyonu hesapla
        float step = cameraSpeed * Time.deltaTime; // Her karede eklenilecek mesafe
        float newY = mainCamera.transform.position.y + step;

        // Hedefi aşmamak için kontrol et
        if(newY >= cameraTargetPositon)
        {
            newY = cameraTargetPositon;
        }

        // Kamerayı yeni pozisyona ayarla
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, newY, mainCamera.transform.position.z);

        yield return null; // Bir sonraki kareyi bekle
    }
    }
}
