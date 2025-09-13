using Godot;
using System;

public static class Assert
{
    public static void IsNoneNode<T>(T node) where T : Node
    {
        if (node == null)
        {
            throw new NullReferenceException($"{typeof(T).Name} node is null");
        }
    }
}