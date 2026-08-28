using UnityEngine;
using TMPro;

public class TaskTimeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeDisplayText;
    [SerializeField] private MixTasksManager mixTasksManager;

    private void Update()
    {
        if (mixTasksManager.isTaskActive)
        {
            float currentTime = GameTimeManager.Instance.GetCurrentTimeInMinutes();
            float dueTimeInMinutes = mixTasksManager.GetTaskDueTimeInMinutes();
            
            float remainingMinutes = dueTimeInMinutes - currentTime;
            
            if (remainingMinutes <= 0)
            {
                timeDisplayText.text = "Görev süresi doldu!";
                return;
            }

            int remainingHours = Mathf.FloorToInt(remainingMinutes / 60);
            int minutesPart = Mathf.FloorToInt(remainingMinutes % 60);

            if (minutesPart < 30)
            {
                minutesPart = 0;
            }
            else
            {
                minutesPart = 30;
            }

            timeDisplayText.text = $"Görevin süresinin dolmasına: {remainingHours} saat {minutesPart} dakika";
        }
        else
        {
            timeDisplayText.text = " ";
        }
    }
} 