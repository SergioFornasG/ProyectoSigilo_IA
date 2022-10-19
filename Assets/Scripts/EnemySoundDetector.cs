using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundDetector : MonoBehaviour
{
 
    private GameObject player;
    private PlayerMovement playerScript;
    public float hearRadius = 40f;
    private float originalHearRadius;

    void Start()
    {

        player = GameObject.Find("JohnLemon");
        playerScript = player.GetComponent <PlayerMovement>();
        originalHearRadius = hearRadius;
    }


    void Update()
    {
        float hearDistance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log("Distancia : " + hearDistance);
        switch (playerScript.playerState)
        {
            case PlayerMovement.State.Run:
                hearRadius = originalHearRadius * 2;
                break;
            case PlayerMovement.State.Walk:
                hearRadius = originalHearRadius;
                break;
            case PlayerMovement.State.Stealth:
                hearRadius = originalHearRadius / 3;
                break;

        }

        if (hearDistance <= hearRadius)
        {
            //Aqui se debe llamar a un componente para que avise a el resto de enemigos cerca del area donde está el jugador.
            Debug.Log("Escuchando con un radio de : " + hearRadius);
        }
    }
}
