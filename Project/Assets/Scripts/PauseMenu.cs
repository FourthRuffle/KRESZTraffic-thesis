using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Canvas Canvas;
    public static PauseMenu Instance; 
    public TextMeshProUGUI statusMessage; 
    void Awake() { Instance = this; }
    void Start()
    {
        Canvas.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }
    public void ToggleMenu()
    {
        Canvas.enabled = !Canvas.enabled;
        if (Canvas.enabled) statusMessage.text = "PAUSED";
    }

    
    public void OpenWithResult(string message, Color color)
    {
        Canvas.enabled = true;
        statusMessage.text = message;
        statusMessage.color = color;
    }
}
