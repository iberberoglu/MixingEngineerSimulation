using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using TMPro;

public class MixerControl : MonoBehaviour
{
    [SerializeField] public Slider slider1;
    [SerializeField] public Slider slider2;
    [SerializeField] public Slider slider3;
    [SerializeField] public Slider slider4;
    [SerializeField] private Button playButton;

    [SerializeField] private Songs songsSO; // Scriptable Object referansı
    [SerializeField] private List<Songs> songList = new List<Songs>(); // Scriptable Object listesi
    [SerializeField] private List<TextMeshProUGUI> channelNames = new List<TextMeshProUGUI>(); // Kanal isimleri
    
    [SerializeField] PlayPauseImageChange playPauseImageChange;
    
    [SerializeField] MeteringDisplay meteringDisplay;

    public EventInstance track1 { get; private set; }
    public EventInstance track2 { get; private set; }
    public EventInstance track3 { get; private set; }
    public EventInstance track4 { get; private set; }


    public bool isPlaying { get; private set; } = false;
    
    [SerializeField] MixTasksManager mixTasksManager;

    void Start()
    {
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
    
    public void setSong(int index)
    {
        songsSO = songList[index];
        
        if (!songsSO.channel1Event.IsNull)
        {
            track1 = RuntimeManager.CreateInstance(songsSO.channel1Event);
            SetVolume(track1, songsSO.channelVolumeStart); // Initialize gain with slider value
            slider1.value = songsSO.channelVolumeStart;
            channelNames[0].text = songsSO.channelNames[0];
        }
        if (!songsSO.channel2Event.IsNull)
        {
            track2 = RuntimeManager.CreateInstance(songsSO.channel2Event);
            SetVolume(track2, songsSO.channelVolumeStart);
            slider2.value = songsSO.channelVolumeStart;
            channelNames[1].text = songsSO.channelNames[1];
        }
        if (!songsSO.channel3Event.IsNull)
        {
            track3 = RuntimeManager.CreateInstance(songsSO.channel3Event);
            SetVolume(track3, songsSO.channelVolumeStart);
            slider3.value = songsSO.channelVolumeStart;
            channelNames[2].text = songsSO.channelNames[2];
        }
        if (!songsSO.channel4Event.IsNull)
        {
            track4 = RuntimeManager.CreateInstance(songsSO.channel4Event);
            SetVolume(track4, songsSO.channelVolumeStart);
            slider4.value = songsSO.channelVolumeStart;
            channelNames[3].text = songsSO.channelNames[3];
        }
        
        meteringDisplay.ResetMetering(track1, track2, track3, track4);
    }

    public void setSongEmpty()
    {
        if (isPlaying)
        {
           PauseTrack(track1);
           PauseTrack(track2);
           PauseTrack(track3);
           PauseTrack(track4); 
        } 
        isPlaying = false;
        playPauseImageChange.SetPlayImage();
        
        
        if (track1.isValid()) track1.release();
        if (track2.isValid()) track2.release();
        if (track3.isValid()) track3.release();
        if (track4.isValid()) track4.release();

        track1 = default;
        track2 = default;
        track3 = default;
        track4 = default;
        
        slider1.value = 50f;
        slider2.value = 50f;
        slider3.value = 50f;
        slider4.value = 50f;

        Debug.Log("Mixer kontrolü sıfırlandı, track'ler temizlendi.");
        
        meteringDisplay.ResetMetering(default, default, default, default);
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
        }
        else
        {
            StartTrack(track1);
            StartTrack(track2);
            StartTrack(track3);
            StartTrack(track4);
            isPlaying = true;
        }
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

        // Slider değerini normalize et ve 0-1 arasında sınırla
        float normalizedValue = Mathf.Clamp01(value / 100f);

        // FMOD parametre aralığına dönüştür
        float volume = Mathf.Lerp(-80f, 10f, normalizedValue);

        // FMOD parametresini ayarla
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
