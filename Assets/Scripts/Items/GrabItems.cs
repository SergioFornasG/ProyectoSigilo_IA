using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItems : MonoBehaviour
{
    AudioSource audioSource;
    int key = 0;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update(){

        if(key == 3){
            //open door
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Key")
        {
            Debug.Log("Grab key");
            audioSource.Play();
            key++;
            Debug.Log("number of keys " + key);
        }
        
            
    }
}