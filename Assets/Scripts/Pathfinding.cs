using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//Lista de listas. Crear una funcion Contains que recorra openNodes y closedNodes mediante for comprobando que gameObject sea el mismo
public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    [SerializeField]private GameObject waypointsGameObject;
    
    private void Awake() => Instance = this;

    public List<GameObject> FindPath(GameObject originWaypoint, GameObject targetWaypoint)
    {
        var startingNode = new Node(originWaypoint);   //Pasamos el gameObject waypoint al constructor del nodo, asi podremos hacer uso de parametros como el fCost o el parent
        var targetNode = new Node(targetWaypoint);
        
        var openNodes = new List<Node>();    //Creara el monticulo que tendra como tamaño maximo el numero de nodos que haya en el mapa
        var closedNodes = new List<Node>();    //Lista de nodos cuya distancia minima ha sido encontrada
        openNodes.Add(startingNode);    //El primer nodo se añade al monticulo y se da comienzo al bucle para encontrar camino

        while (openNodes.Count > 0) //Se seguira ejecutando hasta que ya no haya nodos por visitar (no hay camino) o llegue al objetivo (condicion para ello mas tarde)
        {
            Node currentNode = openNodes[0];
            for (int i = 1; i < openNodes.Count; i ++) {
                if (openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost) {
                    if (openNodes[i].hCost < currentNode.hCost)
                        currentNode = openNodes[i];
                }
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
            
            if (currentNode.thisWaypoint == targetNode.thisWaypoint)  //currentNode.thisWaypoint = targetWaypoint//
            {
                var path = new List<GameObject>();
                while (currentNode.thisWaypoint != startingNode.thisWaypoint)
                {
                    path.Add(currentNode.thisWaypoint);
                    currentNode = currentNode.parent;
                }
                path.Reverse();
                return path;
            }

            foreach (var neighbor in FindNeighbours(currentNode))   //Se movera a traves de los vecinos del actual y hara comprobaciones pertinentes
            {
                if (!ClosedNodesContains(closedNodes, neighbor))    //Si la lista de nodos cerrados ya contiene vecino no tendra un mejor camino asi que no se considera
                {
                    var costToNeighbor = currentNode.gCost + ManhattanDistance(currentNode.thisWaypoint.transform.position, neighbor.thisWaypoint.transform.position); //Distancia de nodo actual a vecino
                    if (!OpenNodesContains(openNodes, neighbor) || costToNeighbor < neighbor.gCost)   //Si se llega a el por primera vez o el camino para llegar es mas corto
                    {
                        neighbor.gCost = costToNeighbor;
                        neighbor.hCost = ManhattanDistance(neighbor.thisWaypoint.transform.position, targetNode.thisWaypoint.transform.position);
                        neighbor.parent = currentNode;
                        
                        if (!OpenNodesContains(openNodes, neighbor))  //Si es la primera vez que se llega al nodo este se añade a la lista de abiertos
                            openNodes.Add(neighbor);
                        /*else 
                            openNodes.UpdateItem(neighbor);*/ //Al haber mejorado el coste puede que deba subir en la jerarquía del monticulo
                    }
                }
            }
        }
        return new List<GameObject>();  //En caso de que no haya encontrado camino (imposible en nuestro juego eso sí), devolverá una lista vacía
    }

    private bool ClosedNodesContains(List<Node> closedNodes, Node node)
    {
        foreach (var iterationNode in closedNodes)
        {
            if (iterationNode.thisWaypoint == node.thisWaypoint)
                return true;
        }
        return false;
    }
    
    private bool OpenNodesContains(List<Node> openNodes, Node node)
    {
        foreach (var iterationNode in openNodes)
        {
            if (iterationNode.thisWaypoint == node.thisWaypoint)
                return true;
        }
        return false;
    }
    
    private List<GameObject> RetracePath(Node startNode, Node endNode)
    {
        var path = new List<GameObject>();
        var currentNode = endNode;  //Se empieza desde el final para ir moviendose a traves de los parents

        while (currentNode != startNode)
        {
            path.Add(currentNode.thisWaypoint);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    } 

    private List<Node> FindNeighbours(Node currentNode)   //Encontrará los waypoints vecinos al actual
    {
        var neighbors = new List<Node>();
        var graphPathing = currentNode.thisWaypoint.GetComponent<GraphPathing>();
        foreach (var arc in graphPathing.arcosDeSalida)
        {
            var node = new Node(arc);
            neighbors.Add(node);
        }
        return neighbors;
    }
    
    private float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
    //Ejemplos de como trabajar con esto
    /*Node node = openNodes.RemoveFirst();
      GraphPathing graphPathing = node.thisWaypoint.GetComponent<GraphPathing>();
      GameObject gameObject = graphPathing.arcosDeSalida[0];
      var position = node.thisWaypoint.transform.position;*/

/*          Node currentNode = openNodes[0];
            for (int i = 1; i < openNodes.Count; i ++) {
                if (openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost) {
                    if (openNodes[i].hCost < currentNode.hCost)
                        currentNode = openNodes[i];
                }
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
            
            
            var currentNode = openNodes.RemoveFirst();  //Se saca el nodo con menor fCost
            Debug.Log(currentNode.thisWaypoint);
            closedNodes.Add(currentNode);   //Al ser el nodo con menor fCost este ya tiene su mejor distancia y se mete al HashSet de cerrados
*/
