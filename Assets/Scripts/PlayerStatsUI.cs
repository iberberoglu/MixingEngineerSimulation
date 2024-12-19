using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider experienceSlider;
    
    public static PlayerStatsUI Instance; // Singleton instance

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
    
    private void Start() {
        UpdateMoneyText();
        UpdateLevelText();
        UpdateExperienceSlider();
    }
    public void UpdateMoneyText()
    {
        float money = PlayerStats.Instance.money;
        moneyText.text = $"Money: {money}$";
    }
    
    public void UpdateLevelText()
    {
        int level = PlayerStats.Instance.level;
        levelText.text = $"Level: {level}";
    }
    
    public void UpdateExperienceSlider()
    {
        int experience = PlayerStats.Instance.experience;
        int experienceRequired = PlayerStats.Instance.experienceRequired;
        
        experienceSlider.maxValue = experienceRequired;
        experienceSlider.value = experience;
    } 
    
}
