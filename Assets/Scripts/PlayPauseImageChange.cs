using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPauseImageChange : MonoBehaviour
{
    [SerializeField] Sprite playImage;
    [SerializeField] Sprite pauseImage;
    [SerializeField] Image imageComponent;
    [SerializeField] MixerControl mixerControl;
    
    [SerializeField] MixTasksManager mixTasksManager;
    
    public void ChangeImage()
    {
        if(mixTasksManager.isTaskSelected)
        {
            if(mixerControl.isPlaying)
            {
                imageComponent.sprite = playImage;
            }
            else
            {
                imageComponent.sprite = pauseImage;    
            }
        }
        else
        {
            Debug.Log("Please select a task first!");
        }
    }
}
