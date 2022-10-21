using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPathing : MonoBehaviour
{
    public List<Collider> arcosDeSalida;
    public List<Collider> arcoDeEntrada;
    public List<Collider> neighbors;
    public bool isPositionAssigned;

    private void Start()
    {
        isPositionAssigned = false;
    }
}
