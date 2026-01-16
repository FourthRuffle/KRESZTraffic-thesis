using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntersectionRoadDirection
{
    public IntersectionDirection direction;
    public IntersectionDirection left;
    public IntersectionDirection right;
    public Vector3 pos;
    public Vector3 rot;

    public IntersectionRoadDirection(IntersectionDirection _dir, IntersectionDirection _left, IntersectionDirection _right, Vector3 _pos, Vector3 _rot)
    {
        direction = _dir; left = _left; right = _right; pos = _pos; rot = _rot;
    }
}

public class IntersectionBuilder : MonoBehaviour
{
    public List<Intersection> intersections;
    public GameObject road;
    Intersection current;

    List<IntersectionRoadDirection> roadDirection = new List<IntersectionRoadDirection>()
    {
        new IntersectionRoadDirection(IntersectionDirection.UP, IntersectionDirection.RIGHT, IntersectionDirection.LEFT, new Vector3(0, 0, 4), new Vector3(0, 180, 0)),
        new IntersectionRoadDirection(IntersectionDirection.DOWN, IntersectionDirection.LEFT, IntersectionDirection.RIGHT, new Vector3(0, 0, -4), Vector3.zero),
        new IntersectionRoadDirection(IntersectionDirection.LEFT, IntersectionDirection.UP, IntersectionDirection.DOWN, new Vector3(-4, 0, 0), new Vector3(0, 90, 0)),
        new IntersectionRoadDirection(IntersectionDirection.RIGHT, IntersectionDirection.DOWN, IntersectionDirection.UP, new Vector3(4, 0, 0), new Vector3(0, -90, 0)),
    };

   
    void Start()
    {
        if (intersections.Count > 0)
        {
            BuildIntersection(intersections[Random.Range(0, intersections.Count)].identifier);
            GenerateVehicles();

            // Now that cars exist, calculate the priority order
            IntersectionSolver.Instance.SetupLevel();
        }
    }

    void BuildIntersection(string id)
    {
        current = intersections.Find(x => x.identifier == id);
        if (current == null) { return; }

        Instantiate(current.baseModel, transform);
        foreach (IntersectionRoad roadData in current.roads)
        {
            IntersectionRoadDirection placeAt = roadDirection.Find(x => x.direction == roadData.direction);
            GameObject newRoad = Instantiate(road, placeAt.pos, Quaternion.Euler(placeAt.rot), transform);
            newRoad.GetComponent<Road>().AssignDirection(current, placeAt);
        }
    }

    void GenerateVehicles()
    {
        int toGenerate = Random.Range(2, current.roads.Count + 1);
        List<Road> roads = FindObjectsOfType<Road>().ToList();
        for (int i = 0; i < toGenerate; i++)
        {
            int randomRoad = Random.Range(0, roads.Count);
            roads[randomRoad].PlaceVehicle();
            roads.RemoveAt(randomRoad);
        }
    }
}