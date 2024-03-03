using Godot;

public partial class MVP : Scene
{
    private Character player;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        BattleController.Instance.OnBattleEnd += OnBattleEnd;

        var spawns = this.GetNodesInChildren<WorldSpawn>();
        spawns.ForEach(x => x.Spawn());

        player = CharacterController.Instance.CreateCharacter(CharacterType.Adventurer);
        PlayerController.Instance.SetTargetCharacter(player);

        player.ThirdPersonVCam.TeleportTo();
    }

    private void OnBattleEnd(EndBattleArgs args)
    {
        CameraBrain.MainCamera.TeleportTo(player.ThirdPersonVCam);
        PlayerController.Instance.SetTargetCharacter(player);
        player.GlobalPosition = Vector3.Zero;
    }
}
