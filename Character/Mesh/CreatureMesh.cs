using Godot;

public partial class CreatureMesh : CharacterMesh
{
    [Export]
    public string MeleeAttackAnimation;

    [Export]
    public string ProjectileAttackAnimation;

    [Export]
    public string HurtAnimation;

    [Export]
    public string DeathAnimation;
}
