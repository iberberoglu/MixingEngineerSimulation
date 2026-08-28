using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] TextMeshProUGUI completeTaskText;
    [SerializeField] TextMeshProUGUI noTasksText; // Yeni eklenen text referansı
    [SerializeField] private Sprite selectedButtonSprite;
    [SerializeField] private Sprite normalButtonSprite;
    [SerializeField] private GameObject canvasToDeactivate; // Yeni eklenen canvas referansı
    [SerializeField] private PlayerController playerController; // Yeni eklenen PlayerController referansı

    [SerializeField] private Button completeTaskButton;
    [SerializeField] EventReference coinCollectedSound;
    [SerializeField] GiveMixTips giveMixTips;
    [SerializeField] ItemCosts itemCosts;
    
    [SerializeField] PlayPauseImageChange playPauseImageChange;
    
    private FMOD.Studio.EventInstance coinCollectedInstance;
    
    public MixTasks selectedTask { get; private set; } = null;

    private Button selectedButton = null; // Seçili olan buton

    private int currentDay = 1; // Mevcut gün
    private int nextTaskDay; // Rastgele bir gün sonra görev eklemek için
    private List<Button> taskButtons = new List<Button>();
    public bool isTaskActive = false;
    private float taskDueTimeInMinutes; // Görevin bitiş zamanı (dakika cinsinden)
    private bool isTaskDue = false;
    
    

    private void Start()
    {
        coinCollectedInstance = RuntimeManager.CreateInstance(coinCollectedSound);
        
        // Tüm görevleri başlangıçta sıfırla
        foreach (var task in mixTasksList)
        {
            task.isCompleted = false;
        }
        
        // İlk görev ekleme
        currentDay = GameTimeManager.Instance.GetCurrentDay();
        UpdateTasks(currentDay);
        
        // Accept butonuna dinleyici ekle
        accceptButton.onClick.AddListener(OnAcceptButtonClicked);

        // Complete Task butonunu başlangıçta deaktif et
        SetCompleteButtonState(false);
        
        Debug.Log($"MixTasksManager Start - Mevcut Gün: {currentDay}, Toplam Görev Sayısı: {mixTasksList.Count}");
    }

    private void SetCompleteButtonState(bool interactable)
    {
        completeTaskButton.interactable = interactable;
        // Butonun rengini ayarla
        Image buttonImage = completeTaskButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (!interactable)
            {
                buttonImage.color = new Color(198f/255f, 198f/255f, 198f/255f);
            }
            else
            {
                buttonImage.color = Color.white;
            }
        }
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
            float currentTimeInMinutes = GameTimeManager.Instance.GetCurrentTimeInMinutes();
            if(currentTimeInMinutes >= taskDueTimeInMinutes)
            {
                OnTaskDurationEnd();
            }
        }

        // Görev durumu mesajlarını güncelle
        UpdateTaskAvailabilityMessage();
    }

    private void UpdateTasks(int day)
    {
        // Her gün yeni bir görev ekle
        MixTasks task = GetRandomTask();
        if (task != null)
        {
            AddTaskButton(task);
            Debug.Log($"Yeni Görev Eklendi: {task.taskName}");
        }
    }

    private MixTasks GetRandomTask()
    {
        Debug.Log($"GetRandomTask başladı - Toplam görev sayısı: {mixTasksList.Count}");
        
        // Her bir görevi kontrol et ve neden elendiğini göster
        foreach (var task in mixTasksList)
        {
            if (task.isCompleted)
            {
                Debug.Log($"Görev '{task.taskName}' tamamlandığı için elendi");
            }
            if (task == selectedTask)
            {
                Debug.Log($"Görev '{task.taskName}' şu an aktif görev olduğu için elendi");
            }
            if (taskButtons.Exists(b => b.GetComponentInChildren<TextMeshProUGUI>().text == task.taskName))
            {
                Debug.Log($"Görev '{task.taskName}' zaten bir butonu olduğu için elendi");
            }
        }

        // Tamamlanmış, aktif veya butonları olan görevleri hariç tut
        List<MixTasks> availableTasks = mixTasksList.FindAll(task => 
            !task.isCompleted && // Tamamlanmış görevleri hariç tut
            task != selectedTask && // Aktif görevi hariç tut
            !taskButtons.Exists(b => b.GetComponentInChildren<TextMeshProUGUI>().text == task.taskName) // Zaten butonu olan görevleri hariç tut
        );

        Debug.Log($"Kullanılabilir görev sayısı: {availableTasks.Count}");
        
        if (availableTasks.Count > 0)
        {
            var selectedTask = availableTasks[Random.Range(0, availableTasks.Count)];
            Debug.Log($"Seçilen görev: {selectedTask.taskName}");
            return selectedTask;
        }
        else
        {
            Debug.LogWarning("Eklenebilecek yeni görev kalmadı!");
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

        // Ana görev metnini ayarla
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = task.taskName;
        }

        // Detay metnini ayarla
        TextMeshProUGUI detailsText = newButton.transform.Find("TaskDetails").GetComponent<TextMeshProUGUI>();
        if (detailsText != null)
        {
            detailsText.text = $"Görev süresi: {task.taskDuration} gün\nÜcret: {task.moneyReward}$";
            Debug.Log($"Details text ayarlandı: {detailsText.text}");
        }
        else
        {
            Debug.LogError("TaskDetails text componenti bulunamadı!");
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
            Debug.LogWarning("Bir görev zaten aktif durumda! Yeni görev seçilemez.");
            return;
        }
        if (selectedTask != null && selectedButton != null)
        {
            // Canvas'ı deaktif et
            if (canvasToDeactivate != null)
            {
                canvasToDeactivate.SetActive(false);
            }

            // Player movement'ı aktif et
            if (playerController != null)
            {
                playerController.SetMovementEnabled(true);
            }

            // Complete Task butonunu aktif et
            SetCompleteButtonState(true);

            // Görev süresini dakika cinsinden hesapla
            float currentTimeInMinutes = GameTimeManager.Instance.GetCurrentTimeInMinutes();
            taskDueTimeInMinutes = currentTimeInMinutes + (selectedTask.taskDuration * 24 * 60); // Günü dakikaya çevir
            
            Debug.Log($"Görev aktif edildi: {selectedTask.taskName} Bitiş zamanı: {taskDueTimeInMinutes} dakika");
            isTaskActive = true;

            // Şarkıyı ayarla mixerde
            mixerControl.setSong(selectedTask.song.songIndex);

            // Görev butonunu kaldır
            RemoveTaskButton(selectedTask);
            
            currentTaskText.text = "Aktif Görev: " + selectedTask.taskName;
            
            selectedButton = null;
            
            completeTaskText.text = "";
            
            // Tolerance area ayarları
            giveMixTips.SetToleranceArea(selectedTask.tolerance, selectedTask.ch1IdealLevel, selectedTask.ch2IdealLevel, selectedTask.ch3IdealLevel, selectedTask.ch4IdealLevel);
            
            // Hoparlör durumunu kontrol et ve ayarla
            if(itemCosts.isPlayerHasSpeaker)
            {
                // StoreManager'dan speaker ayarlarını güncelle
                FindObjectOfType<StoreManager>().UpdateSpeakerSettings();
                giveMixTips.SetMixTipsActive(true);
            }
            else
            {
                giveMixTips.SetMixTipsActive(false);
            }
            
            playPauseImageChange.pleaseSelectTaskPanel.SetActive(false);
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
        // Complete Task butonunu deaktif et
        SetCompleteButtonState(false);

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
            // Complete Task butonunu deaktif et
            SetCompleteButtonState(false);

            mixerControl.setSongEmpty();
            isTaskActive = false; 
            currentTaskText.text = "Aktif Görev: Yok";
            completeTaskText.text = "Görev süresi doldu!";
            
            // Süresi dolsa bile görevi tamamlandı olarak işaretle
            if (selectedTask != null)
            {
                selectedTask.isCompleted = true;
                Debug.Log($"Görev '{selectedTask.taskName}' süresi dolduğu için tamamlandı olarak işaretlendi");
            }
            
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
            float criticalMultiplier = 0f;
            float nonCriticalMultiplier = 0f;

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
                    StartCoroutine(HideCompleteTaskTextAfterDelay(5.0f));
                    
                    // Görevi başarısız olsa da tamamlandı olarak işaretle
                    selectedTask.isCompleted = true;
                    Debug.Log($"Görev '{selectedTask.taskName}' başarısız olarak tamamlandı");
                    
                    return; // Exit the function
                }

                // Calculate reward factor
                float factor = CalculateRewardFactor(sliderValue, idealLevel, selectedTask.tolerance);
                
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
                criticalMultiplier /= criticalChannels;
            }
            if (nonCriticalChannels > 0)
            {
                nonCriticalMultiplier /= nonCriticalChannels;
            }

            // Combine multipliers with weights
            rewardMultiplier = (criticalMultiplier * 0.7f) + (nonCriticalMultiplier * 0.3f);
            Debug.Log($"Reward multiplier: {rewardMultiplier}");
            
            if(rewardMultiplier == 1)
            {
                isFactorTrue = true;
            }

            if (rewardMultiplier > 0f)
            {
                // Apply reward multiplier to XP and money
                int finalXP = Mathf.RoundToInt(selectedTask.experienceReward * rewardMultiplier);
                float finalMoney = Mathf.RoundToInt(selectedTask.moneyReward * rewardMultiplier);

                PlayerStats.Instance.AddExperience(finalXP);
                PlayerStats.Instance.AddMoney(finalMoney);
                
                // Para kazanma sesi çal
                coinCollectedInstance.start();
                
                // Görevin başarı düzeyine göre mesajı belirle
                if(isFactorTrue)
                {
                    completeTaskText.text = selectedTask.successMessagePerfect;
                    StartCoroutine(HideCompleteTaskTextAfterDelay(5.0f));
                }
                else
                {
                    completeTaskText.text = selectedTask.successMessageSemiPerfect;
                    StartCoroutine(HideCompleteTaskTextAfterDelay(5.0f));
                }
            }
            else
            {
                Debug.LogWarning("Task failed! No rewards.");
                completeTaskText.text = selectedTask.failMessage;
                StartCoroutine(HideCompleteTaskTextAfterDelay(5.0f));
            }

            // Her durumda görevi tamamlandı olarak işaretle
            selectedTask.isCompleted = true;
            Debug.Log($"Görev '{selectedTask.taskName}' tamamlandı olarak işaretlendi");

            // Update UI
            PlayerStatsUI.Instance.UpdateLevelText();
            PlayerStatsUI.Instance.UpdateMoneyText();
            PlayerStatsUI.Instance.UpdateExperienceSlider();
        }
    }


    IEnumerator HideCompleteTaskTextAfterDelay(float delay)
    {
        Debug.Log("Coroutine started");
        yield return new WaitForSeconds(delay);
        Debug.Log("Coroutine ended");
        completeTaskText.text = "";
    }


    
    private float CalculateRewardFactor(float sliderValue, float idealLevel, float tolerance)
    {
        float distance = Mathf.Abs(sliderValue - idealLevel);

        if (distance < tolerance / 4)
        {
            return 1f; // Perfect alignment
        }
        else if (distance > tolerance)
        {
            return 0f; // Out of tolerance
        }

        return 1f - (distance / tolerance); // Gradual penalty
    }

    // Yeni method - TaskTimeDisplay için
    public float GetTaskDueTimeInMinutes()
    {
        return taskDueTimeInMinutes;
    }

    private void UpdateTaskAvailabilityMessage()
    {
        if (isTaskActive)
        {
            if (taskButtons.Count == 0)
            {
                noTasksText.text = "Zaten bir görevin var";
            }
            else
            {
                noTasksText.text = "";
            }
        }
        else
        {
            if (taskButtons.Count == 0)
            {
                noTasksText.text = "Şu an yeni görev yok, git biraz uyu!";
            }
            else
            {
                noTasksText.text = "";
            }
        }
    }

}
