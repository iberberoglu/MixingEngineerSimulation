using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance;

    [SerializeField] private TextMeshProUGUI timeText; // UI için zaman göstergesi
    [SerializeField] private float timeScale = 10f; // 1 saniye, oyunda kaç dakika olsun?

    private float currentTimeInMinutes; // Dakika bazlı zamanı takip eder
    private int dayCount = 1; // Gün sayacı

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        currentTimeInMinutes = 360f; // 06:00
    }

    private void Update()
    {
        UpdateGameTime();
        UpdateUI();
    }

    private void UpdateGameTime()
    {
        currentTimeInMinutes += Time.deltaTime * timeScale;

        if (currentTimeInMinutes >= 1440) // 1440 dakika = 1 gün
        {
            currentTimeInMinutes = 0;
            dayCount++;
        }
    }

    private void UpdateUI()
    {
        int hours = (int)(currentTimeInMinutes / 60) % 24;
        int minutes;
        if(currentTimeInMinutes % 60 < 30)
        {
            minutes = 00;
            timeText.text = $"Gün {dayCount}, {hours:D2}:{minutes:D2}";
        }
        else if(currentTimeInMinutes % 60 >= 30)
        {
            minutes = 30;
            timeText.text = $"Gün {dayCount}, {hours:D2}:{minutes:D2}";
        }
    }

    public void AddHours(int hours)
    {
        currentTimeInMinutes += hours * 60;
        if (currentTimeInMinutes >= 1440)
        {
            currentTimeInMinutes -= 1440;
            dayCount++;
        }
    }

    public int GetCurrentDay() => dayCount;
    public float GetCurrentTimeInMinutes() => currentTimeInMinutes;
}
