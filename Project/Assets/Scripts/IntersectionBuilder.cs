using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; // Szükséges a scene nevének lekéréséhez

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
            string difficulty = SceneManager.GetActiveScene().name;
            List<Intersection> filteredIntersections = new List<Intersection>();

            
            if (difficulty == "Easy")
            {
                filteredIntersections = intersections.FindAll(x =>
                    x.roads.Count == 3 &&
                    !x.roads.Exists(r => r.rank != 0));
            }
            else if (difficulty == "Medium")
            {
                filteredIntersections = intersections.FindAll(x =>
                    (x.roads.Count == 3 || x.roads.Count == 4) &&
                    !x.roads.Exists(r => r.rank != 0));
            }
            else if (difficulty == "Hard")
            {
                filteredIntersections = intersections.FindAll(x =>
                    x.roads.Count == 4 &&
                    x.roads.Exists(r => r.rank != 0));
            }
            else 
            {
                filteredIntersections = intersections;
            }

            if (filteredIntersections.Count == 0) filteredIntersections = intersections;

            BuildIntersection(filteredIntersections[Random.Range(0, filteredIntersections.Count)].identifier);
            GenerateVehicles(difficulty);

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

    void GenerateVehicles(string difficulty)
    {
        int toGenerate = 2; // Alapértelmezett

        // --- AUTÓSZÁM BEÁLLÍTÁSA ---
        if (difficulty == "Easy")
        {
            toGenerate = Random.Range(1, 3); // 1 vagy 2 autó
        }
        else if (difficulty == "Medium")
        {
            toGenerate = Random.Range(2, 4); // 2 vagy 3 autó
        }
        else if (difficulty == "Hard")
        {
            toGenerate = 4; // Mindig 4 autó
        }
        else
        {
            toGenerate = Random.Range(2, current.roads.Count + 1);
        }

        // Biztonsági ellenõrzés: ne generáljunk több autót, mint ahány út van
        toGenerate = Mathf.Min(toGenerate, current.roads.Count);

        List<Road> roads = FindObjectsOfType<Road>().ToList();
        for (int i = 0; i < toGenerate; i++)
        {
            if (roads.Count == 0) break;
            int randomRoad = Random.Range(0, roads.Count);
            roads[randomRoad].PlaceVehicle();
            roads.RemoveAt(randomRoad);
        }
    }
}