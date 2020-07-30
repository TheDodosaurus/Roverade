using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Animations;

public class HeapTree
{
    private int[] arr;
    public int size;
    private int swap;

    public HeapTree(int size)
    {
        this.arr = new int[size+1]; // Value 0 will remain 0
        this.size = 0;
    }

    public int Peek()
    {
        if (size == 0) return 0;
        return arr[1];
    }

    public void Insert(int element, float[] values)
    {
        if(size < 0)
        {
            size = 1;
            arr[1] = element;
            return;
        }
        arr[size + 1] = element;
        size++;
        HeapifyUp(size, values);

    }

    public void HeapifyUp(int index, float[] values)
    {
        int parent = index / 2;
        if (index <= 1) return;
        if (values[arr[index]] < values[arr[parent]])
        {
            swap = arr[parent];
            arr[parent] = arr[index];
            arr[index] = swap;
        }

        HeapifyUp(parent, values);
    }

    public void HeapifyDown(int index, float[] values)
    {
        int left = index * 2;
        int right = left + 1;
        int smallest;
        if (size < left) return;
        if (size == left)
        {
            if (values[arr[index]] > values[arr[left]])
            {
                swap = arr[index];
                arr[index] = arr[left];
                arr[left] = swap;
            }
            return;
        }
        if (values[arr[left]] < values[arr[right]]) smallest = left;
        else smallest = right;
        if (values[arr[index]] > values[arr[smallest]])
        {
            swap = arr[index];
            arr[index] = arr[smallest];
            arr[smallest] = swap;
        }
        HeapifyDown(smallest, values);
    }

    public int Extract(float[] values)
    {
        if (size == 0) return -1;
        int extract = arr[1];
        arr[1] = arr[size];
        --size;
        HeapifyDown(1, values);
        return extract;
    }

    public bool Contains(int value)
    {
        for (int i = 1; i <= size; i++)
        {
            if (value == arr[i]) return true;
        }
        return false;
    }
}
