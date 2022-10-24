using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyKey : MonoBehaviour
{
    public GameObject key;
    void Start()
    {
        
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {            Destroy(key);
        }
        
            
    }
}
