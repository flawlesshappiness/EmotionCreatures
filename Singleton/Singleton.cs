using Godot;
using System.Collections.Generic;

public static class Singleton
{
    private static Dictionary<string, Node> _singletons = new();

    public static T CreateInstance<T>(string script_path) where T : Node
    {
        var node = new Node();
        node.Name = typeof(T).Name;

        script_path = $"res://{script_path}.cs";
        var script = GD.Load<Script>(script_path);
        var node_id = node.GetInstanceId();
        node.SetScript(script);
        node = Node.InstanceFromId(node_id) as Node;

        Scene.Root.AddChild(node);

        if (node.TryGetNode<T>(out var instance))
        {
            Debug.Log($"Created instance: {typeof(T).Name}");
            return instance;
        }

        throw new System.NullReferenceException("Failed to get script instance on node");
    }

    public static T CreateSingleton<T>(string script_path) where T : Node
    {
        if (!TryGet<T>(out var singleton))
        {
            var type = typeof(T).Name;
            singleton = CreateInstance<T>(script_path);
            _singletons.Add(type, singleton);

            Debug.Log($"Created singleton: {typeof(T).Name}");
        }

        return singleton;
    }

    public static T LoadInstance<T>(string scene_path) where T : Node
    {
        scene_path = $"res://{scene_path}.tscn";
        var scene = GD.Load(scene_path) as PackedScene;
        var packed_scene = scene.Instantiate();
        Scene.Root.AddChild(packed_scene);
        var script = packed_scene.GetNodeInChildren<T>();
        return script;
    }

    public static T LoadSingleton<T>(string scene_path) where T : Node
    {
        if (!TryGet<T>(out var singleton))
        {
            var type = typeof(T).Name;
            singleton = LoadInstance<T>(scene_path);
            _singletons.Add(type, singleton);
        }

        return singleton;
    }

    public static T Get<T>() where T : Node
    {
        var type = typeof(T).Name;
        return _singletons.ContainsKey(type) ? _singletons[type] as T : throw new System.Exception($"Failed to get singleton with type: {type}");
    }

    public static bool TryGet<T>(out T result) where T : Node
    {
        try
        {
            result = Get<T>();
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
}
