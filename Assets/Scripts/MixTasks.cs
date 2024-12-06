using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MixTasks", menuName = "MixTasks")]
public class MixTasks : ScriptableObject
{
    public string taskName;
    public string taskDescription;
    public float taskDuration;
    public Songs song;
    public bool isCompleted;
    public int experienceReward;
    public float moneyReward;
}
