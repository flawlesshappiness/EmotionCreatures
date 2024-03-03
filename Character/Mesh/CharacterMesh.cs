using Godot;

public partial class CharacterMesh : Node3D
{
    [Export]
    public string IdleAnimation;

    [Export]
    public string MoveAnimation;

    [NodeType(typeof(Skeleton3D))]
    public Skeleton3D Skeleton;

    // THIS METHOD IS UNFINISHED
    public Transform3D GetGlobalBoneTransform(string bone_name)
    {
        Debug.TraceMethod(bone_name);
        Debug.Indent++;

        if (Skeleton == null)
        {
            Debug.LogError("No skeleton on mesh");
            return Transform;
        }

        var bone_index = Skeleton.FindBone(bone_name);
        Debug.LogError($"Bone index: {bone_index}");
        // Check if exists
        var bone_pose = Skeleton.GetBoneGlobalPose(bone_index);
        var transform = Transform * bone_pose;

        Debug.Indent--;
        return transform;
    }
}
