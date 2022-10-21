using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pursuit : MonoBehaviour
{
    public GameObject personaje;
    public float speed;
    public float speedRotation;


    void Update()
    {
        Vector3 direction = personaje.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position + (personaje.transform.forward * 2), speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speedRotation * Time.deltaTime);
    }
}
