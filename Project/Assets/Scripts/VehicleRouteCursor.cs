using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleRouteCursor : MonoBehaviour
{
    public static VehicleRouteCursor cursor;

    public GameObject startCursor;
    public GameObject endCursor;

    GameObject currentStart;
    GameObject currentEnd;

    void Awake()
    {
        cursor = this;
    }

    public void AssignRoute(IntersectionDirection start, IntersectionDirection end)
    {
        List<Road> roads = FindObjectsOfType<Road>().ToList();

        if (currentStart == null) { currentStart = Instantiate(startCursor, roads.Find(x => x.direction.direction == start).startNode.position + Vector3.up, Quaternion.identity); }
        if (currentEnd == null) { currentEnd = Instantiate(endCursor, roads.Find(x => x.direction.direction == end).endNode.position + Vector3.up, Quaternion.identity); }
    }

    public void ClearRoute()
    {
        Destroy(currentStart);
        Destroy(currentEnd);
    }
}
