using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

Collider[] ghostInsideZone;
public LayerMask Ghost;
public string state;
public bool seeked;
public StateMachine s1;
//public bool check =  false;
    // Start is called before the first frame update
    void Start()
    {
        state = "patrol";
        seeked = false;
        /*Debug.Log(message:
        ManagerAlert.Instance.setalert(seeked));*/
    }

    // Update is called once per frame
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
            alert();
            if(seeked == true){
                state = "pursuit";
            }
            break;

        case  "pursuit":
            //Debug.Log("Persiguiendo /estado = " + state);
            //pursuit character
            break;
        
        case  "alerting":
            //Debug.Log("Alertando /estado = " + state);
            //alerta nodos(waypoints) conectados a este
            if(seeked == false){
                state = "return";
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
        //if(check == true){
        ghostInsideZone = Physics.OverlapSphere(transform.position, 5,Ghost);
        //Debug.Log(ghostInsideZone = Physics.OverlapSphere(transform.position, 30,Ghost));

        foreach(var ghost in ghostInsideZone)
        {
            Debug.Log("alerted " + ghost );
            //Debug.Log(ghostInsideZone);
            
            Debug.Log(ManagerAlert.Instance.setalert(seeked));

        }
        //}
    }
    public void setalert(){
        seeked = true;
    }
}
