using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void StartTestMode()
    {
        IntersectionSolver.ResetScore();
        IntersectionSolver.IsTestMode = true;
        IntersectionSolver.CurrentTestLevelIndex = 0;

        IntersectionSolver.GenerateTestSequence();

        string firstScene = IntersectionSolver.TestSequence[0];
        SceneManager.LoadScene(firstScene);
    }


    public void PlayGame(bool isNextStage)
    {
        if (IntersectionSolver.IsTestMode)
        {
            LoadNextTestLevel(isNextStage);
        }
        else
        {

            if (!isNextStage) IntersectionSolver.ResetScore();
            SceneManager.LoadScene("Freeplay");
        }
    }

    private void LoadNextTestLevel(bool isNextStage)
    {
        if (isNextStage)
        {
            IntersectionSolver.CurrentTestLevelIndex++;
        }

        if (IntersectionSolver.CurrentTestLevelIndex < IntersectionSolver.TestSequence.Count)
        {
            string sceneToLoad = IntersectionSolver.TestSequence[IntersectionSolver.CurrentTestLevelIndex];
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        IntersectionSolver.IsTestMode = false;
        IntersectionSolver.ResetScore();
        SceneManager.LoadScene(0);
    }
}