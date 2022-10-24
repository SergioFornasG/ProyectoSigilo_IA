using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

    Collider[] ghostInsideZone;
    public LayerMask Ghost;
    public string state = "patrol";
    public bool seeked; // true si persigue al jugador
    public float alertArea;
    //public Collider g_Collider;
    
    void Start()
    {
        //g_Collider = GetComponent<Collider>();
        state = "patrol";
        seeked = false;
        /*Debug.Log(message:
        ManagerAlert.Instance.setalert(seeked));*/
    }

    /*
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
            //alert();
            state = "pursuit";
            break;

        case  "pursuit":
            //Debug.Log("Persiguiendo /estado = " + state);
            //pursuit character

            if(seeked == false){
               // g_Collider.enabled = false;
            }
            break;
        
        case "return":

            //Debug.Log("Volviendo patrulla /estado = " + state);
            //state = "patrol";
            break;
        /*
        default:
            break;
    }
    }
*/
    //Crea una llamda de alerta hacia la posición waypointDestination alrededor del elemento que contiene este script
    public void alert(Collider waypointDestination){
             
        ghostInsideZone = Physics.OverlapSphere(transform.position, alertArea, Ghost);
        if(ghostInsideZone.Length >= 1)
        {
            foreach (var ghost in ghostInsideZone)
            {

                var movement = ghost.GetComponent<GhostMovement>();
                var theirState = ghost.GetComponent<StateMachine>();
                if(movement != null) movement.setDestinationWaypoint(waypointDestination);
                theirState.SetState("alert");
            }
        }
    }
    /*public void setalert(){
        seeked = true;
    }*/
    public string GetState()
    {
        return state;
    }
    public void SetState(string s)
    {
        this.state = s;
    }
    private void OnTriggerEnter(Collider Ghost)
    {
        if(seeked == false){
        if (Ghost.gameObject.tag == "Ghost") {
             seeked = true;
        }
    }
    }
}
