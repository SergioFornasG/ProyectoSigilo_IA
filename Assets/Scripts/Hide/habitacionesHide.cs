using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class habitacionesHide : MonoBehaviour
{
    private GameObject johnLemon; 


    // Start is called before the first frame update
    void Start()
    {
        johnLemon = GameObject.Find("JohnLemon");

    }

    // Update is called once per frame
    void Update()
    {

       

    }

    private void OnTriggerEnter(Collider other)
    {

            //other.GetComponent<PlayerMovement>().changeHide();
            //Debug.Log("Escondido");




    }

    private void OnTriggerExit(Collider other)
    {

            //Debug.Log("Expuesto");
            //other.GetComponent<PlayerMovement>().changeHide();



    }


}
