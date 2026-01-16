using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntersectionSolver : MonoBehaviour
{
    public static IntersectionSolver Instance;

    // We now track which cars are "Legal" to click right now
    private List<Vehicle> currentLegalMoves = new List<Vehicle>();
    private List<Vehicle> remainingVehicles = new List<Vehicle>();
    private bool isGameOver = false;

    List<LocalDirection> directionOrder = new List<LocalDirection>()
    { LocalDirection.RIGHT, LocalDirection.FORWARD, LocalDirection.LEFT };

    void Awake() { Instance = this; }

    public void SetupLevel()
    {
        remainingVehicles = FindObjectsOfType<Vehicle>().ToList();
        isGameOver = false;
        UpdateLegalMoves();
    }

    void UpdateLegalMoves()
    {
        if (remainingVehicles.Count == 0)
        {
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
    }

    public void OnVehicleClicked(Vehicle clickedVehicle)
    {
        if (isGameOver) return;

        if (currentLegalMoves.Contains(clickedVehicle))
        {
            clickedVehicle.Drive();
            remainingVehicles.Remove(clickedVehicle);

            UpdateLegalMoves();
        }
        else
        {
            isGameOver = true;
            PauseMenu.Instance.OpenWithResult("FAIL!\nIt's not that car's turn.", Color.red);
        }
    }

    IEnumerator ShowResult(bool success)
    {
        yield return new WaitForSeconds(1.5f);
        PauseMenu.Instance.OpenWithResult("SUCCESS!\nIntersection Cleared.", Color.green);
    }
}