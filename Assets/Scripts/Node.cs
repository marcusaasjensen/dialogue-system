using System.Collections.Generic;

public class Node<T>
{
    public T Data { get; set; }
    public List<Node<T>> Children { get; set; }

    public Node(T data, List<Node<T>> children)
    {
        Data = data;
        Children = children;
    }
}