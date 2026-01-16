using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleData
{
    public Vehicle vehicle;
    public LocalDirection direction;
    public int rank;
}

public class IntersectionSolver : MonoBehaviour
{
    List<LocalDirection> directionOrder = new List<LocalDirection>()
    {
        LocalDirection.RIGHT, LocalDirection.FORWARD, LocalDirection.LEFT
    };

    void Start()
    {
        List<Vehicle> vehicles = FindObjectsOfType<Vehicle>().ToList();
        vehicles = vehicles.OrderBy(x => x.Rank).ThenBy(x => GetTurnRank(x)).ThenBy(x => !x.RightHandFirst).ToList();

        for (int i = 0; i < vehicles.Count; i++)
        {
            Vehicle current = vehicles[i];
            Debug.Log($"Vehicle {i + 1}: Start: {current.EntryRoad}, End: {current.ExitRoad}, Direction: {current.LocalDirection}, Road Rank: {current.Rank}");
        }
    }

    int GetTurnRank(Vehicle vehicle) { return directionOrder.FindIndex(x => x == vehicle.LocalDirection); }
}
