using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Collider thisWaypoint;
    
    public float gCost;
    public float hCost;
    public Node parent;

    public float fCost => gCost + hCost;

    public Node(Collider waypoint)
    {
        thisWaypoint = waypoint;
    }
    
    public int CompareTo(Node other)
    {
        var compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(other.hCost);
        return -compare;    //CompareTo devuelve el que tenga numero mas grande, como queremos el más pequeño añadimos el -
    }

    public int HeapIndex { get; set; }
}

/*public int HeapIndex
    {
        get => _heapIndex;
        set => _heapIndex = value;
    }*/
