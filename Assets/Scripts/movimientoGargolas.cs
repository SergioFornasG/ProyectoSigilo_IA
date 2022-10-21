using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimientoGargolas : MonoBehaviour
{
    public int velocidadGiro;
    public float tiempoCambioDireccion;
    public float tiempoCambioParpadeo;
    public GameObject zonaDeteccion;

    Vector3 rotacionPositiva = new Vector3(0f,1f,0f);
    Vector3 rotacionNegativa = new Vector3(0f,-1f,0f);
    Vector3 rotacion;
    float tiempoRotacion;
    float tiempoParpadeo;
    
    void Start()
    {
        rotacion = rotacionPositiva;
        tiempoRotacion = tiempoCambioDireccion;
        tiempoParpadeo = tiempoCambioParpadeo;
    }

    void Update()
    {
        tiempoRotacion -= Time.deltaTime;
        tiempoParpadeo -= Time.deltaTime;
        if(tiempoRotacion <= 0f)
        {
            if(rotacion == rotacionPositiva)
            {
                rotacion = rotacionNegativa;
            }
            else
            {
                rotacion = rotacionPositiva;
            } 
            
            tiempoRotacion = tiempoCambioDireccion; 
        }

        if(tiempoParpadeo <= 0f)
        {
            if(zonaDeteccion.activeSelf)
            {
                zonaDeteccion.SetActive(false);
            }
            else zonaDeteccion.SetActive(true);
            
            tiempoParpadeo = tiempoCambioParpadeo;
        }
        
        transform.Rotate(rotacion * velocidadGiro * Time.deltaTime);
    }
}
