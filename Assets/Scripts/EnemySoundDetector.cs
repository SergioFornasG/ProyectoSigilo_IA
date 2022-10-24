using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundDetector : MonoBehaviour
{

    private GameObject player;
    private PlayerMovement playerScript;
    public float alertRadius;
    public LayerMask waypointMask;
    public StateMachine stateMachine;

    void Start()
    {

        player = GameObject.Find("JohnLemon");
        playerScript = player.GetComponent<PlayerMovement>();
    }


    void Update()
    {
        float hearDistance = Vector3.Distance(transform.position, player.transform.position);
        /*
        switch (playerScript.playerState)
        {
            case PlayerMovement.State.Run:
                hearERadius = originalHearRadius * 2;
                break;
            case PlayerMovement.State.Walk:
                hearERadius = originalHearRadius;
                break;
            case PlayerMovement.State.Stealth:
                hearERadius = originalHearRadius / 3;
                break;

        }*/

        if (hearDistance <= playerScript.GetHearRadius())
        {
            var waypointAlert = Physics.OverlapSphere(transform.position, alertRadius, waypointMask);
            var minDist = 999f;
            Collider waypointTarget = new Collider();
            for (int i = 0; i < waypointAlert.Length; i++)
            {

                var aux = Vector3.Distance(transform.position, waypointAlert[i].transform.position);
                if (aux < minDist)
                {
                    minDist = aux;
                    waypointTarget = waypointAlert[i];
                }
            }
            Debug.Log("waypoint: " + waypointTarget.transform.position.x +" "+waypointTarget.transform.position.z);
            stateMachine.alert(waypointTarget);
            //Aqui se debe llamar a un componente para que avise a el resto de enemigos cerca del area donde está el jugador.
        }
    }
}
