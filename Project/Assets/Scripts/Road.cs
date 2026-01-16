using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum NodeType
{
    START, END
}

public class Road : MonoBehaviour
{
    public IntersectionRoadDirection direction;
    public Transform startNode;
    public Transform endNode;
    public GameObject sign;

    [Header("Cinemachine Paths")]
    public CinemachinePath trackStraight;
    public CinemachinePath trackLeft;
    public CinemachinePath trackRight;

    Intersection intersection;
    public List<GameObject> vehicles;
    List<KeyValuePair<NodeType, Transform>> nodes = new List<KeyValuePair<NodeType, Transform>>();

    void Awake()
    {
        nodes.Add(new KeyValuePair<NodeType, Transform>(NodeType.START, startNode));
        nodes.Add(new KeyValuePair<NodeType, Transform>(NodeType.END, endNode));
    }

    public void AssignDirection(Intersection _intersection, IntersectionRoadDirection _direction)
    {
        intersection = _intersection;
        direction = _direction;
        if (!RoadEqualRanked()) { sign.SetActive(CheckRoadRank() == 0); }
    }

    int CheckRoadRank()
    {
        return intersection.roads.Find(x => x.direction == direction.direction).rank;
    }

    bool RoadEqualRanked()
    {
        return !intersection.roads.Exists(x => x.rank != 0);
    }

    public void PlaceVehicle()
    {
        GameObject prefab = vehicles[Random.Range(0, vehicles.Count)];
        GameObject newVehicle = Instantiate(prefab, startNode.position, startNode.rotation, transform);

        Vehicle vScript = newVehicle.GetComponent<Vehicle>();
        vScript.AssignDirection(intersection, direction);

        CinemachineDollyCart cart = newVehicle.GetComponent<CinemachineDollyCart>();
        switch (vScript.LocalDirection)
        {
            case LocalDirection.FORWARD: cart.m_Path = trackStraight; break;
            case LocalDirection.LEFT: cart.m_Path = trackLeft; break;
            case LocalDirection.RIGHT: cart.m_Path = trackRight; break;
        }
        cart.m_Position = 0;
    }
}