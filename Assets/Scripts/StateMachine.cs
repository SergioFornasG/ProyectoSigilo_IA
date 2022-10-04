using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

public string state;
public bool seeked;
    // Start is called before the first frame update
    void Start()
    {
        state = "patrol";
        seeked = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
    {
        case "patrol":
            Debug.Log("Patrullando /estado = " + state);
            //follow patrol
            //oye ruido

            break;

        case "seek":
            Debug.Log("Buscando /estado = " + state);
            //seek character
            if(seeked == true){
                state = "pursuit";
            }
            break;

        case  "pursuit":
            Debug.Log("Persiguiendo /estado = " + state);
            //pursuit character
            break;
        
        case  "alerting":
            Debug.Log("Alertando /estado = " + state);
            //alerta nodos(waypoints) conectados a este
            if(seeked == false){
                state = "return";
            }
            break;
        
        case "return":
            Debug.Log("Volviendo patrulla /estado = " + state);
            
            //
            //state = "patrol";
            break;

/*
        default:
            Console.WriteLine($"Measured value is {measurement}.");
            break;*/
    }
    }
}
