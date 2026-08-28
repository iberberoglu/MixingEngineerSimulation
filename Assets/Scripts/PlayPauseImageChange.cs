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
    [SerializeField] public GameObject pleaseSelectTaskPanel;
    
    private void Start() {
        pleaseSelectTaskPanel.SetActive(false);
    }
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
            pleaseSelectTaskPanel.SetActive(true);
        }
    }
    
    public void SetPlayImage()
    {
        imageComponent.sprite = playImage;
    }
}
