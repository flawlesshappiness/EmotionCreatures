using Godot;
using System.Text;

public partial class GDHelper
{
    public static T Instantiate<T>(string scene_path) where T : Node
    {
        Debug.TraceMethod(scene_path);

        var prefix = "res://";
        var ext = ".tscn";
        var sb = new StringBuilder();
        if (!scene_path.StartsWith(prefix)) sb.Append(prefix);
        sb.Append(scene_path);
        if (!scene_path.EndsWith(ext)) sb.Append(ext);
        var path = sb.ToString();

        var scene = GD.Load(path) as PackedScene;
        var packed_scene = scene.Instantiate();
        Scene.Root.AddChild(packed_scene);
        var script = packed_scene.GetNodeInChildren<T>();
        return script;
    }
}
