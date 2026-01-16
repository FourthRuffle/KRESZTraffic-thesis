using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntersectionSolver : MonoBehaviour
{
    // Singleton allows Vehicle and PauseMenu to find the Solver easily
    public static IntersectionSolver Instance;

    private List<Vehicle> correctOrder = new List<Vehicle>();
    private int playerStep = 0;
    private bool isGameOver = false;

    // Defines turn priority: Right turns first, then Straight, then Left turns
    List<LocalDirection> directionOrder = new List<LocalDirection>()
    {
        LocalDirection.RIGHT, LocalDirection.FORWARD, LocalDirection.LEFT
    };

    void Awake()
    {
        Instance = this;
    }

    // Called by IntersectionBuilder after all vehicles are spawned
    public void SetupLevel()
    {
        List<Vehicle> vehicles = FindObjectsOfType<Vehicle>().ToList();

        // Sorting Logic: 
        // 1. Road Rank (Priority vs Secondary)
        // 2. Turn Rank (Right > Straight > Left)
        // 3. Right-Hand Rule (Is there a car to your right?)
        correctOrder = vehicles
            .OrderBy(x => x.Rank)
            .ThenBy(x => GetTurnRank(x))
            .ThenBy(x => !x.RightHandFirst)
            .ToList();

        playerStep = 0;
        isGameOver = false;
        Debug.Log("Level Setup: Answer Key generated with " + correctOrder.Count + " vehicles.");
    }

    // Helper to find index of direction in the priority list
    int GetTurnRank(Vehicle vehicle)
    {
        return directionOrder.FindIndex(x => x == vehicle.LocalDirection);
    }

    // Called by Vehicle.cs when the player clicks a car
    public void OnVehicleClicked(Vehicle clickedVehicle)
    {
        if (isGameOver) return;

        // Check if the clicked vehicle is the correct one in the sequence
        if (clickedVehicle == correctOrder[playerStep])
        {
            clickedVehicle.Drive(); // Signal the car to move
            playerStep++;

            // If all cars are clicked correctly
            if (playerStep >= correctOrder.Count)
            {
                StartCoroutine(DelayedResult(true));
            }
        }
        else
        {
            // Wrong choice: End game and show failure
            isGameOver = true;
            PauseMenu.Instance.OpenWithResult("FAIL!\nWrong Priority.", Color.red);
        }
    }

    // Delay showing success so the player can watch the last car drive off
    IEnumerator DelayedResult(bool success)
    {
        yield return new WaitForSeconds(1.5f);
        PauseMenu.Instance.OpenWithResult("SUCCESS!\nCorrect Order.", Color.green);
    }
}