using Godot;
using System.Linq;

public partial class Scene : NodeScript
{
    private bool _initialized;

    public bool IsPaused => GetTree().Paused;

    public SceneData Data { get; private set; }

    public static Scene Current { get; set; }
    public static SceneTree Tree { get; set; }
    public static Window Root { get; set; }
    public static MultiLock PauseLock { get; } = new();
    public static bool AutoSave { get; set; } = true;

    protected virtual void OnInitialize() { }
    protected virtual void OnDestroy() { }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!_initialized)
            Initialize();
    }

    private void Initialize()
    {
        _initialized = true;
        OnInitialize();
    }

    public static T CreateInstance<T>(string path) where T : Scene =>
        Singleton.LoadInstance<T>(path);

    #region DATA
    public virtual void SaveData()
    {
        Debug.Log("Scene.SaveData");
        Debug.Indent++;
        Debug.Indent--;
    }

    public virtual void LoadData()
    {
        Debug.Log("Scene.LoadData");
        Debug.Indent++;

        Data.Nodes.Load();

        Debug.Indent--;
    }

    protected void SaveNode(string path)
    {
        Data.Nodes.Save(GetNode(path), n => n.GetPath());
        Save.Game.Serialize();
    }
    #endregion

    #region SCENE
    public static Scene Goto(string scene_name)
    {
        Debug.Log($"Scene.Goto: {scene_name}");
        Debug.Indent++;

        if (Current != null)
        {
            if (AutoSave)
            {
                Current.SaveData();
            }

            Current.QueueFree();
        }

        Current = CreateInstance<Scene>($"Scenes/{scene_name}");
        Current.Data = GetOrCreateSceneData(scene_name);
        Current.LoadData();
        //Player.LoadData();

        Debug.Indent--;
        return Current;
    }

    public static T Goto<T>() where T : Scene =>
        Goto(typeof(T).Name) as T;

    public void Destroy() => Destroy(this);

    public static void Destroy(Scene scene)
    {
        scene.OnDestroy();
        scene.QueueFree();
    }

    private static SceneData GetOrCreateSceneData(string scene_name)
    {
        var data = Save.Game.Scenes.FirstOrDefault(d => d.SceneName == scene_name);

        if (data == null)
        {
            Debug.Log("New scene data created!");
            data = new SceneData
            {
                SceneName = scene_name,
            };
            Save.Game.Scenes.Add(data);
        }

        return data;
    }
    #endregion
}
