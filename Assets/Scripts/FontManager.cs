using UnityEngine;
using TMPro;

public class FontManager : MonoBehaviour
{
    [SerializeField] TMP_FontAsset newFont; // Kullanmak istediğiniz TextMeshPro Font

    private void Start()
    {
        // Scene'deki tüm TextMeshPro bileşenlerini bul
        TextMeshProUGUI[] textComponents = FindObjectsOfType<TextMeshProUGUI>();

        foreach (var textComponent in textComponents)
        {
            textComponent.font = newFont; // Yeni fontu uygula
        }

        Debug.Log($"Applied font to {textComponents.Length} TextMeshPro components.");
    }
}
