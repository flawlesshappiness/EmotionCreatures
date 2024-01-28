using Godot;

public class NodeData : IDataListItem<Node>
{
    public string Id { get; set; }

    public string Path { get => Id; set => Id = value; }

    public bool Visible { get; set; }

    public void Log()
    {
        Debug.Log($"Path: {Path}");
        Debug.Log($"Visible: {Visible}");
    }

    public void Save(Node reference)
    {
        Debug.Log("NodeData.Save");
        Debug.Indent++;

        if (reference is Node3D n3)
        {
            SaveNode3D(n3);
            Debug.Indent--;
            return;
        }

        if (reference is Node2D n2)
        {
            SaveNode2D(n2);
            Debug.Indent--;
            return;
        }

        Debug.LogError($"Unhandled node of type: {reference.GetType()}");
        Debug.Indent--;
    }

    public void SaveNode3D(Node3D node)
    {
        Debug.Log("NodeData.SaveNode3D");
        Debug.Indent++;

        Visible = node.Visible;

        SaveNode(node);

        Debug.Indent--;
    }

    public void SaveNode2D(Node2D node)
    {
        Debug.Log("NodeData.SaveNode2D");
        Debug.Indent++;

        Visible = node.Visible;

        SaveNode(node);

        Debug.Indent--;
    }

    private void SaveNode(Node node)
    {
        Debug.Log("NodeData.SaveNode");
        Debug.Indent++;

        var path = node.GetPath();
        Path = path.ToString().Replace(Scene.Current.Name, Scene.Current.Data.SceneName);

        Log();

        Debug.Indent--;
    }

    public void Load()
    {
        Debug.Log("NodeData.Load");
        Debug.Indent++;
        Debug.Log($"path: {Path}");

        var node = Scene.Current.GetNode(Path);
        Debug.Log($"node: {node}");

        if (node is Node3D n3)
        {
            LoadNode3D(n3);
        }
        else if (node is Node2D n2)
        {
            LoadNode2D(n2);
        }

        Debug.Indent--;
    }

    public void LoadNode3D(Node3D node)
    {
        Debug.Log("NodeData.LoadNode3D");
        Debug.Indent++;

        node.Visible = Visible;

        LoadNode(node);

        Debug.Indent--;
    }

    public void LoadNode2D(Node2D node)
    {
        Debug.Log("NodeData.LoadNode2D");
        Debug.Indent++;

        node.Visible = Visible;

        LoadNode(node);

        Debug.Indent--;
    }

    private void LoadNode(Node node)
    {
        Log();
    }
}
