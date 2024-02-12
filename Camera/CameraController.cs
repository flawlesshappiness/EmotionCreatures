using Godot;

public partial class CameraController : Node
{
    public static CameraController Instance => Singleton.TryGet<CameraController>(out var instance) ? instance : Create();

    public static CameraController Create() =>
        Singleton.Create<CameraController>($"Camera/{nameof(CameraController)}");

    private Camera3D _camera;
    public Camera3D Camera => _camera ?? (_camera = FindCamera());

    private Camera3D FindCamera()
    {
        Debug.LogMethod();
        Debug.Indent++;

        var camera = Scene.Current.GetNodeInChildren<Camera3D>();
        if (camera == null)
        {
            Debug.LogError("Found no camera");
        }

        Debug.Indent--;
        return camera;
    }
}
