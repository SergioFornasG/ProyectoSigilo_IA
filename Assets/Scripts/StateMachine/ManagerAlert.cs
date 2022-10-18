using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ManagerAlert : MonoBehaviour
{
    public static ManagerAlert Instance;

    private GameObject Ghost1;
    private GameObject Ghost2;
    private GameObject Ghost3;
    public bool seeked; 
    private void Awake() => Instance = this;
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
    /*public void setalert(seeked){
            seeked = Ghost1.GetComponent<StateMachine>();
            
    }*/
}
