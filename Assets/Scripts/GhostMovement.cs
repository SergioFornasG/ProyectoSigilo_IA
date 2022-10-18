using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    private List<GameObject> _tempList;
    
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject waypointTarget;
    private GameObject _previousWaypoint;
    private GraphPathing _graphPathing;   //Se hara un getComponent mas tarde en el Update cuando se cambie a un nuevo waypoint

    //TEST
    [SerializeField] private GameObject originalWaypoint;
    [SerializeField] private GameObject destinationWaypoint;
    private List<GameObject> list;
    public bool isAlert;
    public bool isPatrolling;
    private bool _isPathReadyToCalculate;
    private int _currentWaypointIndex;
    //TEST
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _graphPathing = waypointTarget.GetComponent<GraphPathing>();
        _isPathReadyToCalculate = true;
        _currentWaypointIndex = 0;
    }

    private void Update()
    {
        if (isPatrolling)
            CalculatePatrolPathing();
        else if (!isPatrolling && _isPathReadyToCalculate)   //Solo una vez cuando cambia de patrullando a alerta, asi que calcule el camino
        {
            list = Pathfinding.Instance.FindPath(waypointTarget, destinationWaypoint);
            _isPathReadyToCalculate = false;
        }
        else if (isAlert)
        {
            CalculateAlertPathing();
        }
    }

    private void FixedUpdate()  //En FixedUpdate ya que son movimientos causados por las fisicas
    {
        if (isPatrolling || isAlert)
            PatrolMovement();
    }

    private void CalculateAlertPathing()
    {
        if (_currentWaypointIndex != list.Count && Vector3.Distance(transform.position, waypointTarget.transform.position) < 0.3f)
        {
            waypointTarget = list[_currentWaypointIndex];
            _currentWaypointIndex++;
        }
        DesiredRotation();
    }
    private void CalculatePatrolPathing()
    {
        if (Vector3.Distance(transform.position, waypointTarget.transform.position) < 0.3f) //Cuando llegue a un waypoint
        {
            _previousWaypoint = waypointTarget; //Guarda el waypoint en el que esta para luego poder borrarlo del siguiente y no aparezca como una opcion en el random

            if (_graphPathing.arcosDeSalida.Count == 1) //Es == 1 cuando es el ultimo nodo de un recorrido
                waypointTarget = _graphPathing.arcosDeSalida[0];
            
            else
            {
                _graphPathing.arcosDeSalida.Remove(_graphPathing.arcoDeEntrada[0]); //Se borra el que sea igual al de entrada (del cual vienes) para que no se de la vuelta en mitad de la ruta
                waypointTarget = _graphPathing.arcosDeSalida[Random.Range(0, _graphPathing.arcosDeSalida.Count)];
                _graphPathing.arcosDeSalida.Add(_graphPathing.arcoDeEntrada[0]);    //Lo añade de vuelta para que no se produzcan fallos al volver por ahí en la patrulla
                _graphPathing.arcoDeEntrada.RemoveAt(0);    //Se borra el de entrada ya que al volver por el mismo camino su puesto lo pasará a ocupar otro
            }
            _graphPathing = waypointTarget.GetComponent<GraphPathing>();
            _graphPathing.arcoDeEntrada.Add(_previousWaypoint); //Justo despues de cambiar al graphPathing del siguiente waypoint, se le añade como arcoDeEntrada aquel del cual vienes
        }
        DesiredRotation();
    }

    private void PatrolMovement()
    {
        //if (_isMoving)
        _rigidbody.MovePosition(transform.position + transform.forward * (speed * Time.fixedDeltaTime));
    }

    private void DesiredRotation()
    {
        var relativePosition = waypointTarget.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(relativePosition);   //El objetivo al cual querrá mirar el fantasma
        var currentRotation = transform.localRotation;
        transform.localRotation = Quaternion.Slerp(currentRotation, rotation, Time.deltaTime * turnSpeed);  //Slerp interpola (esféricamente) para crear un rotación fluida
    }
}