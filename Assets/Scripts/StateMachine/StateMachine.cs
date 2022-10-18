using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

//Collider[] ghostInsideZone;
//public LayerMask Ghost;
public string state;
public bool seeked;
Collider g_Collider;
    
    void Start()
    {
        g_Collider = GetComponent<Collider>();
        state = "patrol";
        seeked = false;
        /*Debug.Log(message:
        ManagerAlert.Instance.setalert(seeked));*/
    }

    
    void Update()
    {
        switch (state)
    {
        case "patrol":
            //Debug.Log("Patrullando /estado = " + state);
            //follow patrol
            //oye ruido

            break;

        case "seek":
            //Debug.Log("Buscando /estado = " + state);
            //seek character
            if(seeked == true){
                state = "alerting";
            }
            break;

        case  "alerting":
            //Debug.Log("Alertando /estado = " + state);
            alert();
            state = "pursuit";
            break;

        case  "pursuit":
            //Debug.Log("Persiguiendo /estado = " + state);
            //pursuit character

            if(seeked == false){
                g_Collider.enabled = false;
            }
            break;
        
        case "return":

            //Debug.Log("Volviendo patrulla /estado = " + state);
            //state = "patrol";
            break;
        /*
        default:
            break;*/
    }
    }
    public void alert(){
        //enable collider
        
            g_Collider.enabled = true;
        
        /*
        ghostInsideZone = Physics.OverlapSphere(transform.position, 5,Ghost);
        //Debug.Log(ghostInsideZone = Physics.OverlapSphere(transform.position, 30,Ghost));

        foreach(var ghost in ghostInsideZone)
        {
            Debug.Log("alerted " + ghost );
            //Debug.Log(ghostInsideZone);
            //ManagerAlert.Instance.setalert(seeked);
            //Debug.Log(ManagerAlert.Instance.setalert(seeked));

        }
        */
    }
    /*public void setalert(){
        seeked = true;
    }*/
    private void OnTriggerEnter(Collider Ghost)
    {
        if(seeked == false){
        if (Ghost.gameObject.tag == "Ghost") {
             seeked = true;
        }
    }
    }
}
