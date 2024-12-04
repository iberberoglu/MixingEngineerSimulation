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
    
    public void ChangeImage()
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
}
