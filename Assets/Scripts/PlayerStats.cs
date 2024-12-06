using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 1; // Oyuncunun seviyesi
    public int experience = 0; // Oyuncunun mevcut XP'si
    public float money = 0f; // Oyuncunun mevcut parası
    
    public int experienceRequired; // Bir sonraki seviye için gereken XP

    public static PlayerStats Instance; // Singleton instance

    private void Awake()
    {
        // Eğer zaten bir instance varsa, yok et
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Bu instance'ı koru ve referans olarak ata
        Instance = this;
        DontDestroyOnLoad(gameObject); // Sahne geçişlerinde kaybolmaması için
    }

    /// Seviye atlamak için gereken XP'yi döndürür.
    private int GetExperienceRequiredForNextLevel()
    {
        // Basit bir formül: Gereken XP = Level * Level * 100
        return level * level * 100;
    }

    /// XP ekler ve gerekli seviyeye ulaşıldıysa level artırır.
    public void AddExperience(int amount)
    {
        experience += amount;
        Debug.Log($"XP Eklendi: {amount}. Mevcut XP: {experience}");

        // XP yeterli olduğunda level artır
        CheckLevelUp();
    }

    /// XP kontrol eder ve seviyeyi artırır.
    private void CheckLevelUp()
    {
        experienceRequired = GetExperienceRequiredForNextLevel();

        while (experience >= experienceRequired)
        {
            experience -= experienceRequired; // Gerekli XP'yi düş
            level++; // Level artır
            Debug.Log($"Seviye Atlandı! Yeni Seviye: {level}");

            experienceRequired = GetExperienceRequiredForNextLevel(); // Bir sonraki seviye için gereken XP'yi güncelle
        }
    }
    
    public void AddMoney(float amount)
    {
        money += amount;
        Debug.Log($"Para Eklendi: {amount}. Mevcut Para: {money}");
    }
}
