using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(bool isNextStage)
    {
        // If the button is NOT marked as 'Next Stage', reset the score to 0
        if (!isNextStage)
        {
            IntersectionSolver.ResetScore();
        }

        // Load your game scene (Replace "GameScene" with your actual scene name)
        SceneManager.LoadScene("one");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
