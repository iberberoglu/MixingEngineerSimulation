using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject ePopUp; // E tuşu pop-up objesi
    private InteractableObject currentInteractable; // O anki etkileşimdeki obje
    private bool isCanvasActive = false; // Canvas aktif mi?
    [SerializeField] private PlayerCameraMovement playerCameraMovement;
    [SerializeField] PlayerController playerController; 

    private void Start()
    {
        ePopUp.SetActive(false);
    }

    // Yeni Input System'deki OnInteract action'u buraya bağlanacak
    public void OnInteract()
    {
        if (currentInteractable != null)
        {
            ToggleCanvas();
        }
        if (playerController != null)
        {
            playerController.SetMovementEnabled(!isCanvasActive); // Hareketi kontrol et
        }
        if (playerCameraMovement != null && currentInteractable != null && currentInteractable.isCameraMovementEnabled)
        {
            playerCameraMovement.OnCanvasStateChanged(isCanvasActive); // Kamera durumu değişimi
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        InteractableObject interactable = collision.gameObject.GetComponent<InteractableObject>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            ePopUp.SetActive(true); // "E" tuşu pop-up'ı aç
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        InteractableObject interactable = collision.gameObject.GetComponent<InteractableObject>();
        if (interactable == currentInteractable)
        {
            if (isCanvasActive)
            {
                currentInteractable.CloseCanvas();
                isCanvasActive = false;
            }
            currentInteractable = null;
            ePopUp.SetActive(false); // "E" tuşu pop-up'ı kapat
        }
    }

    private void ToggleCanvas()
    {
        if (currentInteractable != null && currentInteractable.HasCanvas())
        {
            isCanvasActive = !isCanvasActive;

            if (isCanvasActive)
            {
                currentInteractable.OpenCanvas();
                ePopUp.SetActive(false); // Pop-up'ı kapat
            }
            else
            {
                currentInteractable.CloseCanvas();
                ePopUp.SetActive(true); // Pop-up'ı geri getir
            }
        }
    }
}
