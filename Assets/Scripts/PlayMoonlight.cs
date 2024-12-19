using UnityEngine;
using FMODUnity;

public class PlayMoonlight : MonoBehaviour
{
    [SerializeField]
    private EventReference soundEvent;

    private FMOD.Studio.EventInstance eventInstance;
    [SerializeField] InteractPiano interactPiano;

    private void Start()
    {
        // FMOD Event Instance oluştur
        eventInstance = RuntimeManager.CreateInstance(soundEvent);
    }

    void OnInteract()
    {
        if(interactPiano.isPlayerNearby)
        {
            PlaySegment();
        }  
    }

    public void PlaySegment()
    {
        // Oynatmayı durdur ve baştan başlat
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance.setTimelinePosition(0); // Baştan başlat

        eventInstance.start(); // Çalmayı başlat

        // 5 saniye sonra durdur
        Invoke(nameof(StopEvent), 5.0f);
    }

    public void StopEvent()
    {
        // Event’i durdur
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnDestroy()
    {
        // Kaynakları serbest bırak
        eventInstance.release();
    }
}
