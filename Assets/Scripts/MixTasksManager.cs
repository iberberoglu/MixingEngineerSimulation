using System.Collections.Generic;
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
    
    private MixTasks selectedTask = null; // Seçili olan görev
    private Button selectedButton = null; // Seçili olan buton


    private int currentDay = 1; // Mevcut gün
    private int nextTaskDay; // Rastgele bir gün sonra görev eklemek için
    private List<Button> taskButtons = new List<Button>();
    
    public bool isTaskActive = false;
    
    

    private void Start()
    {
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
                if (selectedButton != null)
                {
                    // Önceki seçili butonun seçilme durumunu kaldır
                    selectedButton.interactable = true; // Buton aktif hale getirilir
                    selectedButton.image.color = Color.white; // Buton rengi beyaza döner
                }

                // Yeni seçili butonu ve task'ı ayarla
                selectedTask = task;
                selectedButton = buttonComponent;

                // Seçilen butonun rengini "Selected" duruma getir
                buttonComponent.interactable = false; // Selected state'e geçiş yapar
                selectedButton.image.color = Color.green; // Buton rengi beyaza döner
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
            Debug.Log($"Görev aktif edildi: {selectedTask.taskName}");
            isTaskActive = true;

            // Şarkıyı çalmaya başlat
            mixerControl.setSong(selectedTask.song.songIndex);

            // Görev butonunu kaldır
            RemoveTaskButton(selectedTask);

            // Seçili task ve butonu sıfırla
            selectedTask = null;
            selectedButton = null;
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
        // Görev aktifliğini sıfırla
        

        // MixerControl'deki şarkıları sıfırla
        mixerControl.setSongEmpty();
        isTaskActive = false;

        Debug.Log("Görev tamamlandı. Görev durumu sıfırlandı ve mixer kontrolü temizlendi.");
    }

    // Slider'ları sıfırlamak için bir yardımcı metod


}
