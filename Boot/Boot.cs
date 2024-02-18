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

        SaveDataController.Instance.SaveAll();
    }

    private void InitializeScene()
    {
        Scene.Tree = GetTree();
        Scene.Root = Scene.Tree.Root;
        Scene.PauseLock.OnLocked += () => Scene.Tree.Paused = true;
        Scene.PauseLock.OnFree += () => Scene.Tree.Paused = false;
    }

    private void InitializeViews()
    {
        View.LoadSingleton<DebugView>();
        View.LoadSingleton<GameMenuView>();
        View.LoadSingleton<BattleAnimationView>();
        View.LoadSingleton<BattleView>();
        View.LoadSingleton<TeamView>();
        View.LoadSingleton<CreatureSelectView>();
    }

    private void LoadScene()
    {
        Scene.Goto(Save.Game.Scene);
    }
}
