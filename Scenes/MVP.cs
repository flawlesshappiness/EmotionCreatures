using Godot;

public partial class MVP : Scene
{
    private TopDownCameraFollow camera_follow;
    private Character player;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        BattleController.Instance.OnBattleEnd += OnBattleEnd;

        var spawns = this.GetNodesInChildren<WorldSpawn>();
        spawns.ForEach(x => x.Spawn());

        player = CharacterController.Instance.CreateCharacter(CharacterType.Adventurer);
        camera_follow = this.GetNodeInChildren<TopDownCameraFollow>();
        camera_follow.SetTarget(player);

        PlayerController.Instance.SetTargetCharacter(player);
    }

    private void OnBattleEnd(EndBattleArgs args)
    {
        camera_follow.SetTarget(player);
        PlayerController.Instance.SetTargetCharacter(player);
        player.GlobalPosition = Vector3.Zero;
    }
}
