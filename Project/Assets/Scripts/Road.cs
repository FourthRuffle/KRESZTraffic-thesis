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

    [Header("Signs")]
    public GameObject sign;  // Ez lesz a Fõútvonal tábla (Rank > 0)
    public GameObject sign2; // Ez lesz az Elsõbbségadás kötelezõ tábla (Rank == 0)

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


        if (sign != null) sign.SetActive(false);
        if (sign2 != null) sign2.SetActive(false);

        int myRank = CheckRoadRank();
        bool isEqual = RoadEqualRanked();

        if (!isEqual)
        {
            if (myRank > 0)
            {
                if (sign != null) sign.SetActive(true); 
            }
            else
            {
                if (sign2 != null) sign2.SetActive(true);
            }
        }
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
        newVehicle.name = $"Car_{direction.direction}_{vScript.LocalDirection}";

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