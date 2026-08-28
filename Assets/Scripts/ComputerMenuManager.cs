using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject tasksMenu;
    [SerializeField] private GameObject storeMenu;
    [SerializeField] private GameObject tasksButton;
    [SerializeField] private GameObject storeButton;
    
    private float scaleFactorX = 0.15f;
    private float scaleFactorY = 0.15f;
    Vector3 tasksButtonOriginalScale;
    Vector3 tasksButtonNewScale;
    Vector3 storeButtonOriginalScale;
    Vector3 storeButtonNewScale;
    
    void Start()
    {
        tasksButtonOriginalScale = tasksButton.transform.localScale;
        storeButtonOriginalScale = storeButton.transform.localScale;
        ScaleTasksButton();
        tasksMenu.SetActive(true);
        storeMenu.SetActive(false);
    }
    
    public void SetTasksMenuActive()
    {
        if (storeMenu.activeSelf)
        {
            ScaleTasksButton();
            storeMenu.SetActive(false);
            tasksMenu.SetActive(true);
        }
    }
    
    public void SetStoreMenuActive()
    {
        if (tasksMenu.activeSelf)
        {
            ScaleStoreButton();
            tasksMenu.SetActive(false);
            storeMenu.SetActive(true);
        }
    }
    
    public void ScaleTasksButton()
    {
        storeButton.transform.localScale = storeButtonOriginalScale;
        tasksButtonNewScale = new Vector3(tasksButtonOriginalScale.x + scaleFactorX, tasksButtonOriginalScale.y + scaleFactorY, 1);
        tasksButton.transform.localScale = tasksButtonNewScale;
        
    }
    public void ScaleStoreButton()
    {
        tasksButton.transform.localScale = tasksButtonOriginalScale;
        storeButtonNewScale = new Vector3(storeButtonOriginalScale.x + scaleFactorX, storeButtonOriginalScale.y + scaleFactorY, 1);
        storeButton.transform.localScale = storeButtonNewScale;
        
    }
} 
