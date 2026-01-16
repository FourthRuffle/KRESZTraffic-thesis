using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public enum LocalDirection
{
    FORWARD,
    LEFT,
    RIGHT
}

public class Vehicle : MonoBehaviour
{
    private CinemachineDollyCart dollyCart;
    public IntersectionDirection EntryRoad { get; private set; }
    public IntersectionDirection ExitRoad { get; private set; }
    public LocalDirection LocalDirection { get; private set; }
    public int Rank { get => intersection.roads.Find(x => x.direction == EntryRoad).rank; }
    public bool RightHandFirst { get => HasRightNeighbour(); }

    Intersection intersection;
    IntersectionRoadDirection road;
    List<IntersectionDirection> directions = new List<IntersectionDirection>()
    {
        IntersectionDirection.UP, IntersectionDirection.DOWN, IntersectionDirection.LEFT, IntersectionDirection.RIGHT
    };
    void Start()
    {
        dollyCart = GetComponent<CinemachineDollyCart>();
        // Ensure the cart doesn't move immediately
        dollyCart.m_Speed = 0;
    }
    void Update()
    {
        if (dollyCart != null && dollyCart.m_Speed > 0)
        {
            // Check if we have reached or exceeded the path length
            if (dollyCart.m_Position >= dollyCart.m_Path.PathLength)
            {
                dollyCart.m_Speed = 0; // Stop moving
                Despawn();
            }
        }
    }
    void Despawn()
    {
        Destroy(gameObject, 0.1f);
    }
    public void SetPath(CinemachinePathBase path)
    {
        if (dollyCart == null) dollyCart = GetComponent<CinemachineDollyCart>();
        dollyCart.m_Path = path;
        dollyCart.m_Position = 0; // Start at the beginning of the road
    }

    public void AssignDirection(Intersection _intersection, IntersectionRoadDirection _road)
    {
        intersection = _intersection;
        road = _road;
        EntryRoad = _road.direction;
        ChooseDestination();
    }


    void ChooseDestination()
    {
        List<IntersectionDirection> possible = directions.FindAll(x => x != EntryRoad && IntersectionHasRoad(x));
        ExitRoad = possible[Random.Range(0, possible.Count)];
        CalculateLocalDirection();
    }

    bool IntersectionHasRoad(IntersectionDirection direction)
    {
        return intersection.roads.Exists(x => x.direction == direction);
    }

    void CalculateLocalDirection()
    {
        if (ExitRoad == road.left) { LocalDirection = LocalDirection.LEFT; }
        else if (ExitRoad == road.right) { LocalDirection = LocalDirection.RIGHT; }
        else { LocalDirection = LocalDirection.FORWARD; }
    }

    bool HasRightNeighbour()
    {
        List<Vehicle> vehicles = FindObjectsOfType<Vehicle>().ToList();
        return vehicles.Find(x => x.EntryRoad == road.right);
    }
    void OnMouseDown()
    {
        // Start the car! You can adjust the speed value here
        dollyCart.m_Speed = 5f;
    }

    void OnMouseOver()
    {
        VehicleRouteCursor.cursor.AssignRoute(EntryRoad, ExitRoad);
    }

    void OnMouseExit()
    {
        VehicleRouteCursor.cursor.ClearRoute();
    }
}
