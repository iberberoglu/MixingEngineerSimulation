using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioLevelMeters : MonoBehaviour
{
    [SerializeField] private int sampleSize = 256;

    [SerializeField] AudioSource[] audioSources; // 4 kanalı temsil edecek AudioSource dizisi
    private float[] audioSamples;

    [SerializeField] AudioMixer mixer; // Tek bir AudioMixer kullanacağız

    [Header("Image & Sprites")]
    public Image[] volumeIndicators; // 4 kanalı gösterecek Image bileşenleri (ses seviyesi göstergeleri)
    public Sprite[] masterVolumeSprites; // Ses seviyesine bağlı değişen 14 sprite (tüm kanallar için ortak kullanılıyor)

    void Start()
    {
        audioSamples = new float[sampleSize];
    }

    // Update is called once per frame
    void Update()
    {
        // Tüm kanallar için ses seviyelerini hesapla ve göstergeyi güncelle
        for (int i = 0; i < audioSources.Length; i++)
        {
            // Ses örneklerini al
            audioSources[i].GetOutputData(audioSamples, 0);

            // RMS hesapla
            float rmsValue = 0;
            foreach (var sample in audioSamples)
            {
                rmsValue += sample * sample;
            }
            rmsValue = Mathf.Sqrt(rmsValue / sampleSize);

            // RMS'yi dB'ye çevir
            float dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);
            dbValue = Mathf.Clamp(dbValue, -80, 20);

            // Ses seviyesi göstergesini güncelle
            UpdateVolumeIndicator(dbValue, i);
        }
    }

    private void UpdateVolumeIndicator(float dbValue, int index)
    {
        if (masterVolumeSprites.Length == 0 || index >= volumeIndicators.Length) return;

        // Ses seviyesi aralıklarını belirle
        float rangeStep = 100f / (masterVolumeSprites.Length - 1); // dB aralığını sprite sayısına böler
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt((GetChannelAudioLevel(dbValue, index) + 80) / rangeStep), 0, masterVolumeSprites.Length - 1);

        // Image'ın sprite'ını güncelle
        volumeIndicators[index].sprite = masterVolumeSprites[spriteIndex];
    }

    private float GetChannelAudioLevel(float dbValue, int index)
    {
        string channelName = $"Ch{index + 1}"; // "Ch1", "Ch2", "Ch3", "Ch4" şeklinde kanal isimleri oluşturuyoruz
        mixer.GetFloat(channelName, out float value);

        if (value > 0)
        {
            return dbValue + value;
        }
        else
        {
            return dbValue - Mathf.Abs(value);
        }
    }
}
