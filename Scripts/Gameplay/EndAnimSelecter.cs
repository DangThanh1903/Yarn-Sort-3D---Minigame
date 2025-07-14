using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimSelecter : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    void Awake()
    {
        int randInt = Random.Range(0, transform.childCount);
        foreach (Transform newTransform in transform.GetChild(randInt))
        {
            waypoints.Add(newTransform);
        }
    }
}
