using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float visionRadius;
    public float alertRadius;
    [Range(0, 360)] //restringido visionAngle entre esos valores
    public float visionAngle;
    public LayerMask playerMask;
    public LayerMask wallMask;
    public LayerMask waypointMask;
    public GameObject zonaDeteccion;
    public StateMachine stateMachine;
    
    public float meshResolution;    //Numero de rayos casteados
    public MeshFilter viewMeshFilter;
    private Mesh _viewMesh;
    
    [SerializeField] private int edgeResolveIterations;
    [SerializeField] private float edgeDistanceThreshold;
    
    private void Start()
    {
        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = _viewMesh;
        StartCoroutine(nameof(FindPlayer), .2f);
    }

    private IEnumerator FindPlayer(float wait)
    {
        while (true)
        {
            yield return new WaitForSeconds(wait);
            FindPlayerInView();
        }
    }
    private void LateUpdate()
    {
        //FindPlayerInView();
        DrawFOV();
    }
    
    private void FindPlayerInView()
    {
        var playerInViewRadius = Physics.OverlapSphere(transform.position, visionRadius, playerMask);    //Si jugador esta en radio de posible vision se añade
        if (playerInViewRadius.Length == 1)
        {
            var player = playerInViewRadius[0].transform;
            var directionToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToPlayer) < visionAngle/2) //Que el angulo hecho entre donde mira enemigo y donde se encuentra jugador este dentro de una de las dos mitades que forman fov
            {
                var distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (zonaDeteccion.activeSelf && !Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, wallMask)) //Rayo lanzados desde enemigo hasta jugador, comprueba si hay o no pared en medio
                {
                    var waypointAlert = Physics.OverlapSphere(transform.position, alertRadius, waypointMask);
                    var minDist = 999f;
                    var waypointTarget = transform;
                    for(int i = 0; i < waypointAlert.Length; i++)
                    {

                        var aux = Vector3.Distance(transform.position, waypointAlert[i].transform.position);
                        if (aux < minDist)
                        {
                            minDist = aux;
                            waypointTarget = waypointAlert[i].transform;
                        }
                    }
                    Debug.Log("waypoint: " + waypointTarget.position.x +" "+waypointTarget.position.z);
                    //stateMachine.alert(waypointTarget);
                    

                }
            }
        }
    }
    
    private void DrawFOV() //Casteara varios rayos para poder mostrar la vision cuando estos chocan contra una pared
    {
        var rayCount = Mathf.RoundToInt(visionAngle * meshResolution); //Numero de rayos casteados por grado
        var rayAngleSize = visionAngle / rayCount; //El tamaño de cada quesito formado por los rayos casteados
        var raycastPoints = new List<Vector3>();    //Lista de los puntos que el RaycastInfo hitea
        
        RaycastInfo oldRaycast = new RaycastInfo();

        for (var i = 0; i < rayCount; i++)
        {
            var angle = transform.eulerAngles.y - visionAngle / 2 + rayAngleSize * i;   //Se movera de un lado del fov al otro (izquierda a derecha) en incrementos de rayAngleSize
            RaycastInfo newRaycast = RaycastVision(angle);

            if (i > 0)
            {
                var edgeDistanceThresholdExceeded = Mathf.Abs(oldRaycast.rayDistance - newRaycast.rayDistance) > edgeDistanceThreshold;
                if (oldRaycast.hit != newRaycast.hit || (oldRaycast.hit && newRaycast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldRaycast, newRaycast);
                    if (edge.pointA != Vector3.zero)
                        raycastPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero)
                        raycastPoints.Add(edge.pointB);
                }
            }
            
            raycastPoints.Add(newRaycast.endPoint); //Aquí se añade el punto

            oldRaycast = newRaycast;
        }

        var vertexCount = raycastPoints.Count + 1;  //Los vertices que forman los triangulos de vision seran alla donde choque +1 que es el vertice de origen
        var vertices = new Vector3[vertexCount];    //El array que contendra los vertices como tal
        var triangles = new int[(vertexCount - 2) * 3];   //Los triangulos de vision que se formaran uniendo los vertices. I.e: Dos triangulos [0,1,2,0,2,3] -> (4-2)*3 = 6

        vertices[0] = Vector3.zero; //El vertice de origen. El mesh de vision es un hijo del gameObject, por tanto sus coordenadas seran locales (es decir, no es transform.position)
        for (var i = 0; i < vertexCount - 1; i++)   //-1 porque ya se ha incluido el origen
        {
            vertices[i + 1] = transform.InverseTransformPoint(raycastPoints[i]); //+1 para no sobreescribir el vertices[0]

            if (i < vertexCount - 2)    //Para que no intente añadir fuera del array
            {
                triangles[i * 3] = 0; //El vertice origen para cada triangulo
                triangles[i * 3 + 1] = i + 1; //El segundo vertice
                triangles[i * 3 + 2] = i + 2; //El tercer y ultimo vertice
            }
        }
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
    
    public Vector3 AngleDirection(float angle, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
            angle += transform.eulerAngles.y;   //Convierte a angulo global

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    RaycastInfo RaycastVision(float globalAngle)
    {
        Vector3 direction = AngleDirection(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, visionRadius, wallMask))    //Si el rayo impacta con algo
            return new RaycastInfo(true, hit.point, hit.distance, globalAngle);
        
        return new RaycastInfo(false, transform.position + direction * visionRadius, visionRadius, globalAngle); //Si no impacta
    }
    
    public struct RaycastInfo
    {
        public bool hit;    //Si el rayo choca con algo
        public Vector3 endPoint;    //Punto donde acaba el rayo
        public float rayDistance;
        public float rayAngle;  //El rango desde el cual se disparo el rayo

        public RaycastInfo(bool _hit, Vector3 _endPoint, float _rayDistance, float _rayAngle)
        {
            hit = _hit;
            endPoint = _endPoint;
            rayDistance = _rayDistance;
            rayAngle = _rayAngle;
        }
    }
    
    EdgeInfo FindEdge(RaycastInfo minRaycast, RaycastInfo maxRaycast)
    {
        var minAngle = minRaycast.rayAngle;
        var maxAngle = maxRaycast.rayAngle;
        var minPoint = Vector3.zero;
        var maxPoint = Vector3.zero;

        for (var i = 0; i < edgeResolveIterations; i++)
        {
            var angle = (minAngle + maxAngle) / 2;
            RaycastInfo newRaycast = RaycastVision(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minRaycast.rayDistance - newRaycast.rayDistance) > edgeDistanceThreshold;
            if (newRaycast.hit == minRaycast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newRaycast.endPoint;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newRaycast.endPoint;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }
    
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;
        
        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
/*private bool OpenNodesContains(List<Node> openNodes, Node node)
    {
        foreach (var iterationNode in openNodes)
        {
            if (iterationNode.thisWaypoint == node.thisWaypoint)
                return true;
        }
        return false;
    }*/
