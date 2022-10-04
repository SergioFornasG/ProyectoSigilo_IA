using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrullando : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed;
    public float speedRotation;
    
    int currentWaypointIndex;
    float wRadius = 0.1f;

    void Update()
    {
        Vector3 direction = waypoints[currentWaypointIndex].position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        if(Vector3.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < wRadius)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speedRotation * Time.deltaTime);
    }
}
