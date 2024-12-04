using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class MixerControl : MonoBehaviour
{
    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;
    [SerializeField] private Slider slider3;
    [SerializeField] private Slider slider4;
    [SerializeField] private Button playButton;

    [SerializeField] private Songs songs; // Scriptable Object referansı

    public EventInstance track1 { get; private set; }
    public EventInstance track2 { get; private set; }
    public EventInstance track3 { get; private set; }
    public EventInstance track4 { get; private set; }


    public bool isPlaying { get; private set; } = false;

    void Start()
    {
        // Scriptable Object içeriğini kullanarak EventInstance oluştur
        if (!songs.channel1Event.IsNull) track1 = RuntimeManager.CreateInstance(songs.channel1Event);
        if (!songs.channel2Event.IsNull) track2 = RuntimeManager.CreateInstance(songs.channel2Event);
        if (!songs.channel3Event.IsNull) track3 = RuntimeManager.CreateInstance(songs.channel3Event);
        if (!songs.channel4Event.IsNull) track4 = RuntimeManager.CreateInstance(songs.channel4Event);

        // Play button'a tıklama olayını tanımla
        playButton.onClick.AddListener(TogglePlayPause);

        // Slider'lar için listener ekle
        slider1.onValueChanged.AddListener((value) => SetVolume(track1, value));
        slider2.onValueChanged.AddListener((value) => SetVolume(track2, value));
        slider3.onValueChanged.AddListener((value) => SetVolume(track3, value));
        slider4.onValueChanged.AddListener((value) => SetVolume(track4, value));

        // Slider'ların varsayılan değerlerini ayarla
        slider1.value = 0.5f;
        slider2.value = 0.5f;
        slider3.value = 0.5f;
        slider4.value = 0.5f;
    }

    private void TogglePlayPause()
    {
        if (isPlaying)
        {
            PauseTrack(track1);
            PauseTrack(track2);
            PauseTrack(track3);
            PauseTrack(track4);
            isPlaying = false;
            UpdatePlayButtonUI(); // UI güncelle
        }
        else
        {
            StartTrack(track1);
            StartTrack(track2);
            StartTrack(track3);
            StartTrack(track4);
            isPlaying = true;
            UpdatePlayButtonUI(); // UI güncelle
        }
    }

    private void UpdatePlayButtonUI()
    {
        // Düğme ikonunu değiştirme kodu buraya eklenebilir (örneğin, Play/Pause ikonları arasında geçiş yapın)
    }

    private void StartTrack(EventInstance track)
    {
        if (!track.isValid()) return;

        PLAYBACK_STATE playbackState;
        track.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            track.start();
        }
        else
        {
            track.setPaused(false);
        }
    }

    private void PauseTrack(EventInstance track)
    {
        if (track.isValid())
        {
            track.setPaused(true);
        }
    }

    private void SetVolume(EventInstance track, float value)
    {
        if (!track.isValid()) return;

        // Slider değeri (0-1) aralığından FMOD parametre aralığına (-80, 10) dönüştürülüyor
        float volume = Mathf.LerpUnclamped(-80f, 10f, value);
        volume = Mathf.Clamp(volume, -80f, 10f);
        
        track.setParameterByName("Gain", volume);
    }

    void OnDestroy()
    {
        ReleaseTrack(track1);
        ReleaseTrack(track2);
        ReleaseTrack(track3);
        ReleaseTrack(track4);
    }

    private void ReleaseTrack(EventInstance track)
    {
        if (track.isValid())
        {
            track.release();
        }
    }
}
