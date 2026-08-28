
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject infoPanel; // Bu item ile ilişkilendirilen bilgi paneli
    
    private void Start() {
        
        if(infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}
