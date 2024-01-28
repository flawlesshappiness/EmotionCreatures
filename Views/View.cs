public partial class View : ControlScript
{
    public override void _Ready()
    {
        base._Ready();
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
    }

    private static string GetPath<T>() where T : View
    {
        var type = typeof(T).Name;
        var path = $"{Paths.ViewDirectory}/{type}/{type}";
        return path;
    }

    public static T LoadInstance<T>() where T : View =>
        Singleton.LoadInstance<T>(GetPath<T>());

    public static T LoadSingleton<T>() where T : View =>
        Singleton.LoadSingleton<T>(GetPath<T>());

    public static T Get<T>() where T : View =>
        Singleton.Get<T>();
}
