using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer; // AudioMixer referansı
    [SerializeField] private Slider ch1Slider;
    [SerializeField] private Slider ch2Slider;
    [SerializeField] private Slider ch3Slider;
    [SerializeField] private Slider ch4Slider;
    [SerializeField] private Button toggleButton; // Tüm kanalları kontrol eden buton
    [SerializeField] private AudioSource[] audioSources; // Audio kanalları için AudioSource bileşenleri

    public bool isPlaying = true; // Seslerin çalıp çalmadığını takip eder

    void Start()
    {
        // Slider'lara listener ekle
        ch1Slider.onValueChanged.AddListener(value => SetMixerVolume("Ch1", value));
        ch2Slider.onValueChanged.AddListener(value => SetMixerVolume("Ch2", value));
        ch3Slider.onValueChanged.AddListener(value => SetMixerVolume("Ch3", value));
        ch4Slider.onValueChanged.AddListener(value => SetMixerVolume("Ch4", value));

        // Mevcut değerleri slider'lara aktar
        InitializeSlider(ch1Slider, "Ch1");
        InitializeSlider(ch2Slider, "Ch2");
        InitializeSlider(ch3Slider, "Ch3");
        InitializeSlider(ch4Slider, "Ch4");

        // Butona listener ekle
        toggleButton.onClick.AddListener(ToggleAudio);
    }

    private void SetMixerVolume(string channel, float value)
    {
        mixer.SetFloat(channel, value);
    }

    private void InitializeSlider(Slider slider, string channel)
    {
        if (mixer.GetFloat(channel, out float value))
        {
            slider.value = value;
        }
    }

    private void ToggleAudio()
    {
        isPlaying = !isPlaying;

        foreach (var source in audioSources)
        {
            if (isPlaying)
            {
                source.Play();
            }
            else
            {
                source.Pause();
            }
        }
    }
}
