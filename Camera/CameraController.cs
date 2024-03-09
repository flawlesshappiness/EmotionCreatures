using Godot;

public partial class CameraController : SingletonController
{
    public static CameraController Instance => GetController<CameraController>("Camera");
    public CameraComponentCollection Collection => GetCollection(ResourcePaths.Instance.Collection.CameraComponentCollection);

    private CameraComponentCollection _collection;
    protected CameraComponentCollection GetCollection(string path) => _collection ?? (_collection = LoadCollection(path));
    private CameraComponentCollection LoadCollection(string path) => GD.Load<CameraComponentCollection>(path);

    private CameraBrain _camera;
    public CameraBrain Camera => _camera ?? (_camera = FindCamera());

    public override void Initialize()
    {
        base.Initialize();
        _camera ??= FindCamera();
    }

    private CameraBrain FindCamera()
    {
        Debug.LogMethod();
        Debug.Indent++;

        var camera = Scene.Current.GetNodeInChildren<CameraBrain>();
        if (camera == null)
        {
            camera = CreateCameraBrain();
        }

        Debug.Indent--;
        return camera;
    }

    private CameraBrain CreateCameraBrain()
    {
        Debug.TraceMethod();
        var camera = GDHelper.Instantiate<CameraBrain>(Collection.CameraBrain);
        camera.SetParent(Scene.Root);
        return camera;
    }

    public ThirdPersonVirtualCamera CreateThirdPersonVirtualCamera()
    {
        var vcam = GDHelper.Instantiate<ThirdPersonVirtualCamera>(Collection.ThirdPersonVirtualCamera);
        vcam.SetParent(Scene.Current);
        return vcam;
    }

    public StaticVirtualCamera CreateStaticVirtualCamera()
    {
        var vcam = GDHelper.Instantiate<StaticVirtualCamera>(Collection.StaticVirtualCamera);
        vcam.SetParent(Scene.Current);
        return vcam;
    }
}
