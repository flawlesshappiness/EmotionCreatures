using Godot;
using System.Collections.Generic;

public static partial class NodeExtensions
{
    public static void SetParent(this Node node, Node parent, bool keepGlobalTransform = true)
    {
        if (node.GetParent() == null)
        {
            parent.AddChild(node);
        }
        else
        {
            node.Reparent(parent, keepGlobalTransform);
        }
    }

    public static T GetNodeInChildren<T>(this Node node, string name) where T : Node
    {
        if (node.Name == name) return node as T;

        foreach (var child in node.GetChildren())
        {
            var valid_child = child.GetNodeInChildren<T>(name);
            if (valid_child != null)
                return valid_child;
        }

        return null;
    }

    public static T GetNodeInChildren<T>(this Node node) where T : Node
    {
        if (node.TryGetNode<T>(out var result)) return result;

        foreach (var child in node.GetChildren())
        {
            T script = child.GetNodeInChildren<T>();
            if (script != null)
                return script;
        }

        return null;
    }

    public static List<T> GetNodesInChildren<T>(this Node node) where T : Node
    {
        var list = new List<T>();
        Recursive(node);
        return list;

        void Recursive(Node current)
        {
            if (current.TryGetNode<T>(out var result))
            {
                list.Add(result);
            }

            foreach (var child in current.GetChildren())
            {
                Recursive(child);
            }
        }
    }

    public static T GetNodeInParents<T>(this Node node) where T : Node
    {
        var current = node;
        while (current != null)
        {
            if (current.TryGetNode(out T script))
            {
                return script;
            }

            current = current.GetParent();
        }

        return null;
    }

    public static List<T> GetNodesInParents<T>(this Node node) where T : Node
    {
        var list = new List<T>();
        var current = node;
        while (current != null)
        {
            if (current.TryGetNode(out T script))
            {
                list.Add(script);
            }

            current = current.GetParent();
        }

        return list;
    }

    public static bool TryGetNode<T>(this Node parent, out T script) where T : Node
    {
        try
        {
            script = parent.GetNode<T>(parent.GetPath());
            return script != null;
        }
        catch
        {
            script = null;
            return false;
        }
    }

    public static bool TryGetNode<T>(this Node parent, string path, out T script) where T : Node
    {
        try
        {
            script = parent.GetNodeOrNull<T>(path);
            return script != null;
        }
        catch
        {
            script = null;
            return false;
        }
    }
}