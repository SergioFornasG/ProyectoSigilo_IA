using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPathing : MonoBehaviour
{
    public List<GameObject> arcosDeSalida;
    public List<GameObject> arcoDeEntrada;
    public List<GameObject> neighbors;
    private bool _isPositionAssigned;

    private void Start()
    {
        _isPositionAssigned = false;
    }
}
