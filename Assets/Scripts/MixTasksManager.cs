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

    private int currentDay = 1; // Mevcut gün
    private int nextTaskDay; // Rastgele bir gün sonra görev eklemek için
    private List<Button> taskButtons = new List<Button>();
    
    public bool isTaskSelected = false;
    
    

    private void Start()
    {
        // İlk gün kontrolü ve görev ekleme
        nextTaskDay = currentDay; // İlk görev ekleme günü belirlenir
        UpdateTasks(GameTimeManager.Instance.GetCurrentDay());
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
        if(buttonComponent != null && !isTaskSelected)
        {
            buttonComponent.onClick.AddListener(() => OnTaskButtonClicked(task, buttonComponent));
        }
        else
        {
            Debug.Log("You already selected a task!");    
        }
        
        // Listeye ekle
        taskButtons.Add(buttonComponent);
    }

    private void OnTaskButtonClicked(MixTasks task, Button clickedButton)
    {
        Debug.Log($"Görev Seçildi: {task.taskName}");
        isTaskSelected = true;

        // Butonu devre dışı bırak
        clickedButton.interactable = false;

        mixerControl.setSong(task.song.songIndex);
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
}
