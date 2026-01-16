using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Canvas Canvas;
    public static PauseMenu Instance; // Allows other scripts to find this one
    public TextMeshProUGUI statusMessage; // Drag your result text here
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

    // Called by the Solver when the game ends
    public void OpenWithResult(string message, Color color)
    {
        Canvas.enabled = true;
        statusMessage.text = message;
        statusMessage.color = color;
    }
}
