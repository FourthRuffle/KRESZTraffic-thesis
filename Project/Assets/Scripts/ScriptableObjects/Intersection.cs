using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum IntersectionDirection
{
    UP, DOWN, LEFT, RIGHT
}

[System.Serializable]
public class IntersectionRoad
{
    public IntersectionDirection direction;
    [Range(0, 3)] public int rank;
}

[CreateAssetMenu(fileName = "New Intersection", menuName = "Custom/New Intersection")]
public class Intersection : ScriptableObject
{
    public string identifier;
    public List<IntersectionRoad> roads;
    public GameObject baseModel;
}
