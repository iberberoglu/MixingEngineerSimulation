using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MixTasksManager : MonoBehaviour
{
    [SerializeField] private List<MixTasks> mixTasksList = new List<MixTasks>();
    [SerializeField] private GameObject taskButtonPrefab; // Button Prefab'i
    [SerializeField] private Transform tasksContainer; // Butonların ekleneceği Grid Layout
    [SerializeField] private MixerControl mixerControl;
    [SerializeField] private int randomTaskDayMin = 1;
    [SerializeField] private int randomTaskDayMax = 3;
    [SerializeField] private Button accceptButton;
    [SerializeField] TextMeshProUGUI currentTaskText;
    [SerializeField] private Sprite selectedButtonSprite;
    [SerializeField] private Sprite normalButtonSprite;

    [SerializeField] EventReference coinCollectedSound;
    [SerializeField] TextMeshProUGUI completeTaskText;
    
    [SerializeField] GiveMixTips giveMixTips;
    [SerializeField] ItemCosts itemCosts;
    
    private FMOD.Studio.EventInstance coinCollectedInstance;
    
    private MixTasks selectedTask = null; // Seçili olan görev
    private Button selectedButton = null; // Seçili olan buton

    private int currentDay = 1; // Mevcut gün
    private int nextTaskDay; // Rastgele bir gün sonra görev eklemek için
    private List<Button> taskButtons = new List<Button>();
    public bool isTaskActive = false;
    private int taskDueDay;
    private bool isTaskDue = false;
    
    

    private void Start()
    {
        coinCollectedInstance = RuntimeManager.CreateInstance(coinCollectedSound);
        // İlk gün kontrolü ve görev ekleme
        nextTaskDay = currentDay; // İlk görev ekleme günü belirlenir
        UpdateTasks(GameTimeManager.Instance.GetCurrentDay());
        
        // Accept butonuna dinleyici ekle
        accceptButton.onClick.AddListener(OnAcceptButtonClicked);
    }

    private void Update()
    {
        // Gün değişti mi kontrol et
        int newDay = GameTimeManager.Instance.GetCurrentDay();
        if (newDay > currentDay)
        {
            currentDay = newDay;
            UpdateTasks(currentDay);
        }
        
        // Görev süresi doldu mu kontrol et
        if(isTaskActive)
        {
            if(currentDay == taskDueDay)
            {
                OnTaskDurationEnd();
            }
        }
    }

    private void UpdateTasks(int day)
    {
        // Sadece belirlenen gün geldiğinde yeni bir görev ekle
        if (day == nextTaskDay)
        {
            // Rastgele bir görev seç
            MixTasks task = GetRandomTask();
            if (task != null)
            {
                AddTaskButton(task);
                nextTaskDay = CalculateNextTaskDay(); // Bir sonraki görev günü belirle
                Debug.Log($"Yeni Görev Eklendi: {task.taskName}, Sonraki Görev Günü: {nextTaskDay}");
            }
        }
    }

    private MixTasks GetRandomTask()
    {
        // Henüz eklenmemiş bir rastgele görev seç
        List<MixTasks> remainingTasks = mixTasksList.FindAll(t => !taskButtons.Exists(b => b.GetComponentInChildren<TextMeshProUGUI>().text == t.taskName));
        if (remainingTasks.Count > 0)
        {
            return remainingTasks[Random.Range(0, remainingTasks.Count)];
        }
        else
        {
            Debug.LogWarning("Tüm görevler eklenmiş!");
            return null;
        }
    }

    private int CalculateNextTaskDay()
    {
        // Mevcut günden itibaren 1 ile 5 gün arasında bir süre belirle
        return currentDay + Random.Range(randomTaskDayMin, randomTaskDayMax);
    }

    private void AddTaskButton(MixTasks task)
    {
        // Buton oluştur ve container'a ekle
        GameObject newButton = Instantiate(taskButtonPrefab, tasksContainer);
        Button buttonComponent = newButton.GetComponent<Button>();

        // Buton metnini ayarla
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = task.taskName;
        }

        // Butona tıklama olayını ekle
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() =>
            {
                if (isTaskActive)
                {
                    // Bir görev zaten aktif durumda, uyarı mesajı göster
                    Debug.LogWarning("Bir görev zaten aktif durumda! Yeni görev seçilemez.");
                    return;
                }

                // Eski seçili butonu sıfırla
                if (selectedButton != null)
                {
                    selectedButton.interactable = true; // Eski butonu tekrar aktif hale getir
                    selectedButton.image.sprite = normalButtonSprite; // Normal sprite'a döndür
                }

                // Yeni seçili butonu ve task'ı ayarla
                selectedTask = task;
                selectedButton = buttonComponent;

                // Seçilen butonun durumunu güncelle
                buttonComponent.interactable = false; // Buton pasif hale getir
                selectedButton.image.sprite = selectedButtonSprite; // Seçilen sprite'ı uygula

                Debug.Log($"Task seçildi: {task.taskName}, ancak henüz aktif değil. Accept butonuna basılmalı.");
            });
        }

        // Listeye ekle
        taskButtons.Add(buttonComponent);
    }

    
    private void OnAcceptButtonClicked()
    {
        if (isTaskActive)
        {
            // Bir görev zaten aktif durumda, uyarı mesajı göster
            Debug.LogWarning("Bir görev zaten aktif durumda! Yeni görev seçilemez.");
            return;
        }
        if (selectedTask != null && selectedButton != null)
        {
            // Görev süresini ata
            taskDueDay = currentDay + selectedTask.taskDuration;
            
            Debug.Log($"Görev aktif edildi: {selectedTask.taskName} Son gün: {taskDueDay}");
            isTaskActive = true;

            // Şarkıyı ayarla mixerde
            mixerControl.setSong(selectedTask.song.songIndex);

            // Görev butonunu kaldır
            RemoveTaskButton(selectedTask);
            
            currentTaskText.text = "Aktif Görev: " + selectedTask.taskName;
            
            selectedButton = null;
            
            completeTaskText.text = "";
            
            giveMixTips.SetToleranceArea(selectedTask.tolerance, selectedTask.ch1IdealLevel, selectedTask.ch2IdealLevel, selectedTask.ch3IdealLevel, selectedTask.ch4IdealLevel);
            
            if(itemCosts.isPlayerHasSpeaker)
            {
                giveMixTips.SetMixTipsActive(true);
            }
            else
            {
                giveMixTips.SetMixTipsActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Hiçbir görev seçilmedi. Accept butonuna basılmadan önce bir görev seçmelisiniz.");
        }
    }

    public void RemoveTaskButton(MixTasks task)
    {
        // Görev tamamlandıysa veya silinmesi gerekiyorsa, butonu kaldır
        Button buttonToRemove = taskButtons.Find(b => b.GetComponentInChildren<TextMeshProUGUI>().text == task.taskName);
        if (buttonToRemove != null)
        {
            taskButtons.Remove(buttonToRemove);
            Destroy(buttonToRemove.gameObject);
        }
    }
    
    public void OnCompleteTaskButtonClicked()
    {  
        // Görevi tamamla Ödülleri al.
        CompleteTask();
        
        // MixerControl'deki şarkıları sıfırla
        mixerControl.setSongEmpty();
        isTaskActive = false;
        
        currentTaskText.text = "Aktif Görev: Yok";
                
        // Seçili task sıfırla
        selectedTask = null;
        
        giveMixTips.SetMixTipsActive(false);
    }

    public void OnTaskDurationEnd()
    {
        isTaskDue = true;
        if(isTaskDue)
        {
            mixerControl.setSongEmpty();
            isTaskActive = false; 
            currentTaskText.text = "Aktif Görev: Yok";
            Debug.Log("Görev süresi doldu.");
            selectedTask = null;
            giveMixTips.SetMixTipsActive(false);
        }
        
        isTaskDue = false;
    }
    
    private void CompleteTask()
    {
        if (selectedTask != null)
        {
            // Initialize multipliers
            float rewardMultiplier = 0f;
            float criticalMultiplier = 0f; // For channels that "must change"
            float nonCriticalMultiplier = 0f; // For other channels

            // Track the count of critical and non-critical channels
            int criticalChannels = 0;
            int nonCriticalChannels = 0;
            
            bool isFactorTrue = false; 

            // Iterate through each channel and calculate reward factors
            for (int i = 0; i < 4; i++)
            {
                float sliderValue = 0f;
                float idealLevel = 0f;
                bool isCritical = selectedTask.isChHasToChange[i];

                // Assign slider and ideal values dynamically based on index
                switch (i)
                {
                    case 0:
                        sliderValue = mixerControl.slider1.value;
                        idealLevel = selectedTask.ch1IdealLevel;
                        break;
                    case 1:
                        sliderValue = mixerControl.slider2.value;
                        idealLevel = selectedTask.ch2IdealLevel;
                        break;
                    case 2:
                        sliderValue = mixerControl.slider3.value;
                        idealLevel = selectedTask.ch3IdealLevel;
                        break;
                    case 3:
                        sliderValue = mixerControl.slider4.value;
                        idealLevel = selectedTask.ch4IdealLevel;
                        break;
                }

                // Check if the channel is out of tolerance
                float distance = Mathf.Abs(sliderValue - idealLevel);
                if (distance > selectedTask.tolerance)
                {
                    // Fail the task immediately if any channel is out of tolerance
                    Debug.LogWarning($"Channel {i + 1} is out of tolerance. Task failed.");
                    
                    completeTaskText.text = selectedTask.failMessage;
                    
                    return; // Exit the function
                }

                // Calculate reward factor
                float factor = CalculateRewardFactor(sliderValue, idealLevel, selectedTask.tolerance, isCritical);
                
                if(factor == 1)
                {
                    isFactorTrue = true;
                }
                
                // Assign factor to the correct multiplier
                if (isCritical)
                {
                    criticalMultiplier += factor;
                    criticalChannels++;
                }
                else
                {
                    nonCriticalMultiplier += factor;
                    nonCriticalChannels++;
                }
            }

            // Avoid division by zero
            if (criticalChannels > 0)
            {
                criticalMultiplier /= criticalChannels; // Average out critical multipliers
            }
            if (nonCriticalChannels > 0)
            {
                nonCriticalMultiplier /= nonCriticalChannels; // Average out non-critical multipliers
            }

            // Combine multipliers with weights
            rewardMultiplier = (criticalMultiplier * 0.7f) + (nonCriticalMultiplier * 0.3f);
            Debug.Log($"Reward multiplier: {rewardMultiplier}");

            if (rewardMultiplier > 0f)
            {
                // Apply reward multiplier to XP and money
                int finalXP = Mathf.RoundToInt(selectedTask.experienceReward * rewardMultiplier);
                float finalMoney = Mathf.RoundToInt(selectedTask.moneyReward * rewardMultiplier);

                PlayerStats.Instance.AddExperience(finalXP);
                PlayerStats.Instance.AddMoney(finalMoney);
                
                // Görevin başarı düzeyine göre mesajı belirle
                if(isFactorTrue)
                {
                    completeTaskText.text = selectedTask.successMessagePerfect;
                }
                else
                {
                    completeTaskText.text = selectedTask.successMessageSemiPerfect;
                }
                
            }
            else
            {
                Debug.LogWarning("Task failed! No rewards.");
                completeTaskText.text = selectedTask.failMessage;
            }

            // Update UI
            PlayerStatsUI.Instance.UpdateLevelText();
            PlayerStatsUI.Instance.UpdateMoneyText();
            PlayerStatsUI.Instance.UpdateExperienceSlider();

            // Mark task as completed
            selectedTask.isCompleted = true;
        }
}





    
    private float CalculateRewardFactor(float sliderValue, float idealLevel, float tolerance, bool isCritical)
    {
        float distance = Mathf.Abs(sliderValue - idealLevel);

        // High penalty for critical channels
        if (isCritical)
        {
            if (distance < tolerance / 4)
            {
                return 1f; // Perfect match
            }
            else if (distance > tolerance)
            {
                return 0f; // Completely out of tolerance
            }
            return 1f - (distance / tolerance); // Gradual penalty
        }
        else
        {
            // Lower impact for non-critical channels
            if (distance < tolerance / 4)
            {
                return 0.5f; // Perfect match, but less impact
            }
            else if (distance > tolerance)
            {
                return 0f; // Out of tolerance
            }
            return 0.5f - (distance / (2 * tolerance)); // Reduced penalty for non-critical
        }
    }


}
