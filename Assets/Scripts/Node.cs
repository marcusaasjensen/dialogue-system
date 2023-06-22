using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    public T Value { get; set; }
    public List<Node<T>> Children { get; set; }

    public Node(T value, List<Node<T>> children)
    {
        Value = value;
        Children = children;
    }
}