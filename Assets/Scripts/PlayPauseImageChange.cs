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
        if(mixTasksManager.isTaskActive)
        {
            if(mixerControl.isPlaying)
            {
                imageComponent.sprite = playImage;
                Debug.Log("Play");
            }
            else
            {
                imageComponent.sprite = pauseImage;    
                Debug.Log("Pause");
            }
        }
        else
        {
            Debug.Log("Please select a task first!");
        }
    }
    
    public void SetPlayImage()
    {
        imageComponent.sprite = playImage;
    }
}
