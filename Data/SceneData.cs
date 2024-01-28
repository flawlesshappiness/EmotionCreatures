using Godot;

public class SceneData
{
    public string SceneName { get; set; }

    public DataList<NodeData, Node> Nodes { get; set; } = new();
}
