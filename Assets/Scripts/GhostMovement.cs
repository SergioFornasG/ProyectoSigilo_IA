using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float waypointDetectionRadius;

    public Collider player;
    public LayerMask waypointMask;
    private Rigidbody _rigidbody;
    public StateMachine stateMachine;
    [SerializeField] private Collider waypointTarget;
    private Collider _lastPatrolWaypoint;
    private Collider _previousWaypoint;
    private GraphPathing _graphPathing;   //Se hara un getComponent mas tarde en el Update cuando se cambie a un nuevo waypoint

    //TEST
    public Collider destinationWaypoint;
    private List<Collider> _pathList;
    /*public bool isAlert;
    public bool isPatrolling;
    public bool isChasing;
    private bool _isReturningFromAlarm;
    public bool _isReturningFromChase;*/
    
    private bool _isPathReadyToCalculate;   //Para que calcule el pathfinding una sola vez al cambiar al modo de alerta
    private int _currentWaypointIndex;  //Para saber en que elemento de la lista del pathfinding se encuentra
    private bool _isAlertPathReached;   //Para que pueda llegar al ultimo punto del camino sin quedarse parado en el anterior
    private bool _isOverwatching;   //Para que cuando llegue al destino de la alerta no haga DesiredRotation()
    private bool _isLastWaypointAssigned;   //Para que solo se guarde el ultimo punto de la patrulla la primera vez que deje de estar en patrulla
    private bool _hasReturnFromChaseStarted;
    private GraphPathing _destinationGraphPathing;  //Para poder acceder a isPositionAssigned
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _graphPathing = waypointTarget.GetComponent<GraphPathing>();
        _isPathReadyToCalculate = true;
        _isOverwatching = false;
        _isLastWaypointAssigned = false;
        _hasReturnFromChaseStarted = false;
        //_isReturningFromChase = false;
        _currentWaypointIndex = 0;
    }

    private void Update()
    {
        if (stateMachine.GetState() != "patrol" && !_isLastWaypointAssigned)
        {
            _lastPatrolWaypoint = waypointTarget; //Guarda ultimo waypoint de la patrulla para que puede volver alli cuando termine de estar alerta
            _isLastWaypointAssigned = true;
        }

        if (stateMachine.GetState() == "patrol")
            CalculatePatrolPathing();
        
        else if (stateMachine.GetState() == "alert" && _isPathReadyToCalculate)   //Solo una vez cuando cambia de patrullando a alerta, asi que calcule el camino
        {
            Debug.Log("entra");
            _currentWaypointIndex = 0;
            _pathList = Pathfinding.Instance.FindPath(waypointTarget, destinationWaypoint);
            for (int i = 0; i < _pathList.Count; i++)
            {
                Debug.Log(_pathList[i]);
            }
            _destinationGraphPathing = destinationWaypoint.GetComponent<GraphPathing>();
            if (_destinationGraphPathing.isPositionAssigned)
            {
                if (_pathList.Count > 1)    //Para que no se salga del rango
                    _pathList.Remove(_pathList[_pathList.Count - 1]);   //Quita el ultimo elemento para que no pueda haber dos fantasmas en un mismo waypoint
                _destinationGraphPathing = _pathList[_pathList.Count - 1].GetComponent<GraphPathing>(); //Aqui el _pathList.Count - 1 no equivale al mismo de la linea de arriba ya que ese se ha borrado
            }
            _destinationGraphPathing.isPositionAssigned = true;
            _isAlertPathReached = false;    //Se vuelve a poner aqui a falso para que luego se ponga true al llegar al final del pathing
            _isPathReadyToCalculate = false;
        }
        else if (!_isPathReadyToCalculate && stateMachine.GetState() != "alert")
        {
            _currentWaypointIndex = 0;
            _pathList = Pathfinding.Instance.FindPath(waypointTarget, _lastPatrolWaypoint);
            /*for (int i = 0; i < _pathList.Count; i++)
            {
                Debug.Log(_pathList[i]);
            }*/

            speed = 2;
            //Debug.Log("speed2");
            _isPathReadyToCalculate = true;
        }
        else if (stateMachine.GetState() == "returnFromPursuit" && !_hasReturnFromChaseStarted)
        {
            Debug.Log("feferfewa");
            _hasReturnFromChaseStarted = true;
            var waypointChase = Physics.OverlapSphere(transform.position, waypointDetectionRadius, waypointMask);
            var minDist = 999f;
            var possibleWaypoint = new Collider();
            for(var i = 0; i < waypointChase.Length; i++)
            {

                var aux = Vector3.Distance(transform.position, waypointChase[i].transform.position);
                if (aux < minDist)
                {
                    minDist = aux;
                    possibleWaypoint = waypointChase[i];
                }
            }
            waypointTarget = possibleWaypoint;
            _currentWaypointIndex = 0;
            _pathList = Pathfinding.Instance.FindPath(waypointTarget, _lastPatrolWaypoint);
        }
        else if (stateMachine.GetState() == "alert")
        {
            CalculateAlertPathing();
        }
        else if (stateMachine.GetState() == "returnFromAlert" || stateMachine.GetState() == "returnFromPursuit")
        {
            CalculateReturnToPathing();
        }
        else if (stateMachine.GetState() == "pursuit")
        {
            CalculatePursuit();
            
        }
    }

    private void FixedUpdate()  //En FixedUpdate ya que son movimientos causados por las fisicas
    {
        if (stateMachine.GetState() != "pursuit")
            PatrolMovement();
    }

    private void CalculateAlertPathing()
    {
        if (_currentWaypointIndex != _pathList.Count && Vector3.Distance(transform.position, waypointTarget.transform.position) < 0.3f)
        {
            waypointTarget = _pathList[_currentWaypointIndex];
            _currentWaypointIndex++;
        }

        if (_currentWaypointIndex == _pathList.Count && Vector3.Distance(transform.position, waypointTarget.transform.position) < 0.3f && !_isAlertPathReached)
        {
            _isOverwatching = true;
            transform.LookAt(destinationWaypoint.transform);    //Para mirar hacia el waypoint cercano al jugador en vez de poder quedarse mirando a la pared
            _isAlertPathReached = true;
            _destinationGraphPathing.isPositionAssigned = false;    //Lo vuelve a poner a falso para que se pueda poner verdadero cuando se vuelva a llamar una alerta
            speed = 0;
            StartCoroutine(AlertTimer());
        }
        if (!_isOverwatching)
            DesiredRotation();
    }

    private void CalculateReturnToPathing()
    {
        if (_currentWaypointIndex != _pathList.Count && Vector3.Distance(transform.position, waypointTarget.transform.position) < 0.3f)
        {
            waypointTarget = _pathList[_currentWaypointIndex];
            _currentWaypointIndex++;
        }

        if (_currentWaypointIndex == _pathList.Count && Vector3.Distance(transform.position, waypointTarget.transform.position) < 0.3f)
        {
            if (stateMachine.GetState() == "returnFromAlert" || stateMachine.GetState() == "returnFromPursuit")
            {
                stateMachine.SetState("patrol");
                /*_isReturningFromAlarm = false;
                _isReturningFromChase = false;*/
                _isLastWaypointAssigned = false;
            }
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
                _graphPathing.arcosDeSalida.Add(_graphPathing.arcoDeEntrada[0]);    //Lo a??ade de vuelta para que no se produzcan fallos al volver por ah?? en la patrulla
                _graphPathing.arcoDeEntrada.RemoveAt(0);    //Se borra el de entrada ya que al volver por el mismo camino su puesto lo pasar?? a ocupar otro
            }
            _graphPathing = waypointTarget.GetComponent<GraphPathing>();
            _graphPathing.arcoDeEntrada.Add(_previousWaypoint); //Justo despues de cambiar al graphPathing del siguiente waypoint, se le a??ade como arcoDeEntrada aquel del cual vienes
        }
        DesiredRotation();
    }

    private void CalculatePursuit()
    {
        var direction = player.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, player.transform.position) < 1f)
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + player.transform.forward * 2, speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);
    }

    private void PatrolMovement()
    {
        _rigidbody.MovePosition(transform.position + transform.forward * (speed * Time.fixedDeltaTime));
    }

    private void DesiredRotation()
    {
        var relativePosition = waypointTarget.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(relativePosition);   //El objetivo al cual querr?? mirar el fantasma
        var currentRotation = transform.localRotation;
        transform.localRotation = Quaternion.Slerp(currentRotation, rotation, Time.deltaTime * turnSpeed);  //Slerp interpola (esf??ricamente) para crear un rotaci??n fluida
    }

    public void setDestinationWaypoint(Collider wp)
    {
        this.destinationWaypoint = wp;
    }

    private IEnumerator AlertTimer()
    {
        yield return new WaitForSeconds(5f);
        stateMachine.SetState("returnFromAlert");
        _isOverwatching = false;
    }
}