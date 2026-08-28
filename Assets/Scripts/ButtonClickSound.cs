using UnityEngine;
using FMODUnity;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private EventReference clickSound;
    private FMOD.Studio.EventInstance clickInstance;
    private Button button;

    private void Start()
    {
        // FMOD event instance'ını oluştur
        clickInstance = RuntimeManager.CreateInstance(clickSound);
        
        // Bu GameObject'teki butonu al
        button = GetComponent<Button>();
        
        // Butona click sound listener'ı ekle
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        // Ses instance'ını başlat
        clickInstance.start();
    }

    private void OnDestroy()
    {
        // Listener'ı kaldır
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
        
        // Instance'ı temizle
        clickInstance.release();
    }
} 