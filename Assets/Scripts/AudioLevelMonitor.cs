using UnityEngine;
using UnityEngine.UI; // UI için gerekli
using System.Collections.Generic; // Kuyruk yapısı için gerekli

public class AudioLevelMonitor : MonoBehaviour
{
    [SerializeField] int sampleSize = 256; // Örnek boyutu
    [SerializeField] int averageWindowSize = 10; // Ortalama için alınacak veri sayısı
    private float[] audioSamples;
    private Queue<float> averageWindow; // Hareketli ortalama kuyruğu
    private float movingAverage;

    [Header("Image & Sprites")]
    public Image masterVolumeComponent; // Ses seviyesi göstergesi için Image
    public Sprite[] masterVolumeSprites; // Ses seviyesine bağlı değişen 14 sprite

    void Start()
    {
        audioSamples = new float[sampleSize];
        averageWindow = new Queue<float>();
    }

    void Update()
    {
        // Anlık ses verisini al
        AudioListener.GetOutputData(audioSamples, 0);

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

        // Hareketli ortalama için değeri ekle
        averageWindow.Enqueue(dbValue);
        if (averageWindow.Count > averageWindowSize)
        {
            averageWindow.Dequeue(); // Eski değerleri çıkar
        }

        // Hareketli ortalama hesapla
        movingAverage = 0;
        foreach (float value in averageWindow)
        {
            movingAverage += value;
        }
        movingAverage /= averageWindow.Count;

        // Ses seviyesi göstergesini güncelle
        UpdateVolumeIndicator();

        Debug.Log($"Stabilize edilmiş dB seviyesi: {movingAverage}");
    }

    private void UpdateVolumeIndicator()
    {
        if (masterVolumeSprites.Length == 0) return;

        // Ses seviyesi aralıklarını belirle
        float rangeStep = 100f / (masterVolumeSprites.Length - 1); // dB aralığını sprite sayısına böler
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt((movingAverage + 80) / rangeStep), 0, masterVolumeSprites.Length - 1);

        // Image'ın sprite'ını güncelle
        masterVolumeComponent.sprite = masterVolumeSprites[spriteIndex];
    }
}
