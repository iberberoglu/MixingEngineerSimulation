using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private GameObject canvasToOpen; // Bu objeye özel açılacak canvas
    [SerializeField] public bool isCameraMovementEnabled = false; // Kamera hareketi aktif mi?

    public void OpenCanvas()
    {
        if (canvasToOpen != null)
        {
            canvasToOpen.SetActive(true); // Canvas'ı aç
        }
    }

    public void CloseCanvas()
    {
        if (canvasToOpen != null)
        {
            canvasToOpen.SetActive(false); // Canvas'ı kapat
        }
    }

    public bool HasCanvas()
    {
        return canvasToOpen != null; // Canvas atanmış mı?
    }
}
