using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntersectionSolver : MonoBehaviour
{
    public static IntersectionSolver Instance;

    // STATIC: This stays in memory even when the scene reloads
    public static int TotalScore = 0;

    private List<Vehicle> currentLegalMoves = new List<Vehicle>();
    private List<Vehicle> remainingVehicles = new List<Vehicle>();
    private bool isGameOver = false;

    List<LocalDirection> directionOrder = new List<LocalDirection>()
    { LocalDirection.RIGHT, LocalDirection.FORWARD, LocalDirection.LEFT };

    void Awake() { Instance = this; }

    // This method is called by MainMenu to wipe the score for a brand new game
    public static void ResetScore()
    {
        TotalScore = 0;
        Debug.Log("<color=white><b>Score Reset to 0 for New Game</b></color>");
    }

    public void SetupLevel()
    {
        remainingVehicles = FindObjectsOfType<Vehicle>().ToList();
        isGameOver = false;

        // --- INITIAL REPORTING ---
        Debug.Log("<color=cyan><b>--- INITIAL TRAFFIC SITUATION ---</b></color>");
        Debug.Log($"<color=orange><b>CURRENT SESSION SCORE: {TotalScore}</b></color>");

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
            // Start the result sequence for success
            StartCoroutine(ShowResult(true));
            return;
        }

        // 1. Sort the remaining cars to find the priority
        var sorted = remainingVehicles
            .OrderBy(v => v.Rank)
            .ThenBy(v => directionOrder.FindIndex(d => d == v.LocalDirection))
            .ThenBy(v => !v.RightHandFirst)
            .ToList();

        Vehicle bestVehicle = sorted[0];

        // 2. Identify the "Legal Group"
        currentLegalMoves = remainingVehicles.Where(v =>
            v.Rank == bestVehicle.Rank &&
            v.LocalDirection == bestVehicle.LocalDirection &&
            v.RightHandFirst == bestVehicle.RightHandFirst
        ).ToList();

        // --- GROUP REPORTING ---
        string legalNames = string.Join(", ", currentLegalMoves.Select(v => v.name));
        Debug.Log($"<color=yellow><b>NEXT MOVE:</b></color> Any of these: [{legalNames}]");
    }

    public void OnVehicleClicked(Vehicle clickedVehicle)
    {
        if (isGameOver) return;

        if (currentLegalMoves.Contains(clickedVehicle))
        {
            Debug.Log($"<color=green>CORRECT:</color> {clickedVehicle.name} is moving.");

            clickedVehicle.Drive();
            remainingVehicles.Remove(clickedVehicle);

            // Log the remaining order after the move
            string remaining = remainingVehicles.Count > 0
                ? string.Join(" -> ", remainingVehicles.Select(v => v.name))
                : "None";
            Debug.Log($"Remaining Vehicles: {remaining}");

            UpdateLegalMoves();
        }
        else
        {
            isGameOver = true;
            Debug.LogError($"<color=red>WRONG MOVE!</color> {clickedVehicle.name} tried to go out of turn.");
            // Start the result sequence for failure
            StartCoroutine(ShowResult(false));
        }
    }

    IEnumerator ShowResult(bool success)
    {
        if (success)
        {
            TotalScore++;
            Debug.Log($"<color=green>Point Added! New Total: {TotalScore}</color>");
        }

        // Wait so the player sees the last car drive off
        yield return new WaitForSeconds(1.5f);

        // Prepare the message for the PauseMenu
        string msg = success ? $"SUCCESS!\nScore: {TotalScore}" : $"FAIL!\nFinal Score: {TotalScore}";
        Color col = success ? Color.green : Color.red;

        PauseMenu.Instance.OpenWithResult(msg, col);
    }
}