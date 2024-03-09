using Godot;

public partial class Boot : Node
{
    private bool _initialized;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!_initialized)
            Initialize();
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        long id = what;
        if (id is NotificationWMCloseRequest or NotificationCrash or NotificationExitTree)
        {
            Debug.WriteLogsToPersistentData();
        }
    }

    private void Initialize()
    {
        _initialized = true;
        InitializeScene();
        InitializeViews();
        LoadScene();
        InitializeControllers();
        Game.RegisterDebugActions();

        SaveDataController.Instance.SaveAll();
    }

    private void InitializeScene()
    {
        Debug.LogMethod();
        Debug.Indent++;

        Scene.Tree = GetTree();
        Scene.Root = Scene.Tree.Root;
        Scene.PauseLock.OnLocked += () => Scene.Tree.Paused = true;
        Scene.PauseLock.OnFree += () => Scene.Tree.Paused = false;

        Debug.Indent--;
    }

    private void InitializeViews()
    {
        Debug.LogMethod();
        Debug.Indent++;

        View.LoadSingleton<DebugView>();
        View.LoadSingleton<GameMenuView>();
        View.LoadSingleton<BattleAnimationView>();
        View.LoadSingleton<BattleView>();
        View.LoadSingleton<TeamView>();
        View.LoadSingleton<CreatureSelectView>();

        Debug.Indent--;
    }

    private void InitializeControllers()
    {
        Debug.LogMethod();
        Debug.Indent++;

        CameraController.Instance.Initialize();

        Debug.Indent--;
    }

    private void LoadScene()
    {
        Debug.LogMethod();
        Debug.Indent++;

        Scene.Goto(Save.Game.Scene);

        Debug.Indent--;
    }
}
