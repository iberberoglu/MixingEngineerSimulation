using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MixTasks", menuName = "MixTasks")]
public class MixTasks : ScriptableObject
{
    public string taskName;
    public string taskDescription;
    public int taskDuration;
    public Songs song;
    public bool isCompleted;
    public int experienceReward;
    public float moneyReward;

    public float ch1IdealLevel;
    public float ch2IdealLevel;
    public float ch3IdealLevel;
    public float ch4IdealLevel;
    
    public List<bool> isChHasToChange = new List<bool>();
    public float tolerance;
    
    public string successMessagePerfect;
    public string successMessageSemiPerfect;
    public string failMessage;
}
