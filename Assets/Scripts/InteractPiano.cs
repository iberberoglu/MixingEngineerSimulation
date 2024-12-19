using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPiano : MonoBehaviour
{
    public bool isPlayerNearby = false;
    [SerializeField] PlayMoonlight playMoonlight;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        isPlayerNearby = true;
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        playMoonlight.StopEvent();
        isPlayerNearby = false;
    }
}
