using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

public String state;
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
        case == "patrol":
            Console.WriteLine("Patrullando /estado = " + state);
            //follow patrol
            //oye ruido

            break;

        case == "seek":
            Console.WriteLine("Buscando /estado = " + state);
            //seek character
            if(seeked == true){
                state == "pursuit";
            }
            break;

        case == "pursuit":
            Console.WriteLine("Persiguiendo /estado = " + state);
            //pursuit character
            break;
        
        case == "alerting":
            Console.WriteLine("Alertando /estado = " + state);
            //alerta nodos(waypoints) conectados a este
            if(seeked == false){
                state == "return";
            }
            break;
        
        case == "return":
            Console.WriteLine("Volviendo patrulla /estado = " + state);
            
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
