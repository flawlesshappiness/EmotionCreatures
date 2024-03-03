using Godot;

[GlobalClass]
public partial class CameraComponentCollection : Resource
{
    [Export(PropertyHint.File)]
    public string CameraBrain { get; set; }
    [Export(PropertyHint.File)]
    public string ThirdPersonVirtualCamera { get; set; }
    [Export(PropertyHint.File)]
    public string StaticVirtualCamera { get; set; }
}
