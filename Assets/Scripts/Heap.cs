using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    private T[] _items;
    private int _currentItemCount;

    public Heap(int maxHeapSize)
    {
        _items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item;
        SortUp(item);
        _currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = _items[0];
        _currentItemCount--;
        _items[0] = _items[_currentItemCount];  //Se coje el ultimo del monticulo y se pone primero (funcionamiento basico de Heaps)
        _items[0].HeapIndex = 0;
        SortDown(_items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);   //Solo SortUp ya que en A* como mucho solo cambiaremos la prioridad a mejor (si distancia es mejor por otro camino)
    }

    public int Count => _currentItemCount;

    public bool Contains(T item)
    {
        return Equals(_items[item.HeapIndex], item);    //Si el item es igual a un item que haya en el arbol con su mismo index. O sea, te dice si existe o no dentro del Heap
    }

    private void SortDown(T item)
    {
        while (true)
        {
            var childIndexLeft = item.HeapIndex * 2 + 1;
            var childIndexRight = item.HeapIndex * 2 + 2;
            var swapIndex = 0;

            if (childIndexLeft < _currentItemCount) //Si tiene hijo por la izquierda
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < _currentItemCount)    //Si tiene hijo por la derecha
                {
                    if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0)  //Izquierdo tiene menos prioridad
                        swapIndex = childIndexRight;
                }

                if (item.CompareTo(_items[swapIndex]) < 0)  //Padre menos prioridad que hijo con mas prioridad de los dos
                    Swap(item, _items[swapIndex]);
                else   //Padre tiene mas prioridad que los hijos y por tanto se queda igual
                    return; 
            }
            else       //No tiene hijos por tanto se queda como esta
                return;
        }
    }

    private void SortUp(T item)
    {
        var parentIndex = (item.HeapIndex - 1) / 2;

        while (true)    //Se ejecutara hasta que el padre sea hundido a la profundidad necesaria
        {
            T parentItem = _items[parentIndex];
            if (item.CompareTo(parentItem) > 0) //Tiene hijo mayor prioridad, se cambia de posicion con el padre
            {
                Swap(item, parentItem);
            }
            else
                break;
        }
    }

    private void Swap(T itemA, T itemB)
    {
        _items[itemA.HeapIndex] = itemB;
        _items[itemB.HeapIndex] = itemA;
        var itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
