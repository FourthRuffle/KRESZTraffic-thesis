using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntersectionSolver : MonoBehaviour
{
    public static IntersectionSolver Instance;

    public static int TotalScore = 0;
    public static int TestScore = 0;
    public static int CurrentTestLevelIndex = 0;
    public static bool IsTestMode = false;

    public static List<string> TestSequence = new List<string>();
    private static int totalTestStages = 30;

    private List<Vehicle> currentLegalMoves = new List<Vehicle>();
    private List<Vehicle> remainingVehicles = new List<Vehicle>();
    private bool isGameOver = false;

    List<LocalDirection> directionOrder = new List<LocalDirection>()
    { LocalDirection.RIGHT, LocalDirection.FORWARD, LocalDirection.LEFT };

    void Awake() { Instance = this; }

    public static void ResetScore()
    {
        TotalScore = 0;
        TestScore = 0;
        CurrentTestLevelIndex = 0;
        Debug.Log("<color=white><b>Scores and Progress Reset.</b></color>");
    }

    public static void GenerateTestSequence()
    {
        TestSequence.Clear();
        for (int i = 0; i < 10; i++) TestSequence.Add("Easy");
        for (int i = 0; i < 10; i++) TestSequence.Add("Medium");
        for (int i = 0; i < 10; i++) TestSequence.Add("Hard");

        System.Random rng = new System.Random();
        TestSequence = TestSequence.OrderBy(x => rng.Next()).ToList();
    }

    public void SetupLevel()
    {
        remainingVehicles = FindObjectsOfType<Vehicle>().ToList();
        isGameOver = false;

        string activeScene = SceneManager.GetActiveScene().name;
        Debug.Log($"<color=magenta><b>ACTIVE SCENE: {activeScene}</b></color>");

        if (IsTestMode)
        {
            Debug.Log($"<color=cyan>--- TEST MODE: Level {CurrentTestLevelIndex + 1}/{totalTestStages} ---</color>");
            Debug.Log($"<color=orange>Current Test Score: {TestScore}</color>");
        }
        else
        {
            Debug.Log($"<color=cyan>--- FREEPLAY MODE ---</color>");
            Debug.Log($"<color=orange>Current Session Score: {TotalScore}</color>");
        }

        foreach (Vehicle v in remainingVehicles)
        {
            Debug.Log($"Vehicle: {v.name} | From: {v.EntryRoad} | To: {v.ExitRoad} | Turn: {v.LocalDirection} | Rank: {v.Rank}");
        }

        UpdateLegalMoves();
    }

    void UpdateLegalMoves()
    {
        if (remainingVehicles.Count == 0)
        {
            Debug.Log("<color=green><b>ALL CARS CLEARED SUCCESSFULLY!</b></color>");
            StartCoroutine(ShowResult(true));
            return;
        }

        var sorted = remainingVehicles
            .OrderBy(v => v.Rank)
            .ThenBy(v => directionOrder.FindIndex(d => d == v.LocalDirection))
            .ThenBy(v => !v.RightHandFirst)
            .ToList();

        Vehicle bestVehicle = sorted[0];

        currentLegalMoves = remainingVehicles.Where(v =>
            v.Rank == bestVehicle.Rank &&
            v.LocalDirection == bestVehicle.LocalDirection &&
            v.RightHandFirst == bestVehicle.RightHandFirst
        ).ToList();

        string legalNames = string.Join(", ", currentLegalMoves.Select(v => v.name));
        Debug.Log($"<color=yellow><b>NEXT LEGAL MOVE(S):</b></color> [{legalNames}]");
    }

    public void OnVehicleClicked(Vehicle clickedVehicle)
    {
        if (isGameOver) return;

        if (currentLegalMoves.Contains(clickedVehicle))
        {
            Debug.Log($"<color=green>CORRECT:</color> {clickedVehicle.name} is driving.");
            clickedVehicle.Drive();
            remainingVehicles.Remove(clickedVehicle);

            string remaining = remainingVehicles.Count > 0
                ? string.Join(" -> ", remainingVehicles.Select(v => v.name))
                : "None";
            Debug.Log($"Remaining in this intersection: {remaining}");

            UpdateLegalMoves();
        }
        else
        {
            isGameOver = true;
            Debug.LogError($"<color=red>WRONG MOVE!</color> {clickedVehicle.name} violated priority rules.");
            StartCoroutine(ShowResult(false));
        }
    }

    IEnumerator ShowResult(bool success)
    {
        if (success)
        {
            if (IsTestMode) TestScore++;
            else TotalScore++;
        }

        yield return new WaitForSeconds(1.5f);

        if (IsTestMode)
        {
            if (CurrentTestLevelIndex >= totalTestStages - 1)
            {
                float passThreshold = totalTestStages * 0.9f;
                bool passed = TestScore >= passThreshold;

                string finalStatus = passed ? "TEST PASSED!" : "TEST FAILED";
                PauseMenu.Instance.OpenWithResult($"{finalStatus}\nFinal: {TestScore}/{totalTestStages}",
                                                  passed ? Color.green : Color.red);
                IsTestMode = false;
            }
            else
            {
                string status = success ? "STAGE CLEAR!" : "STAGE FAILED!";
                string progressMsg = $"{status}\nQuestion: {CurrentTestLevelIndex + 1}/{totalTestStages}\nScore: {TestScore}";

                PauseMenu.Instance.OpenWithResult(progressMsg, success ? Color.white : Color.red);
            }
        }
        else
        {
            string msg = success ? $"SUCCESS!\nScore: {TotalScore}" : $"FAIL!\nFinal Score: {TotalScore}";
            PauseMenu.Instance.OpenWithResult(msg, success ? Color.green : Color.red);
        }
    }
}