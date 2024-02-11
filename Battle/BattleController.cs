using Godot;
using System.Collections;

public partial class BattleController : Node
{
    public static BattleController Instance => Singleton.TryGet<BattleController>(out var instance) ? instance : Create();

    public static BattleController Create() =>
        Singleton.CreateSingleton<BattleController>($"Battle/{nameof(BattleController)}");

    private BattleAnimationView AnimationView { get; set; }

    public override void _Ready()
    {
        base._Ready();

        AnimationView = View.LoadSingleton<BattleAnimationView>();
    }

    public void StartBattle(ArenaType arena_type)
    {
        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            Debug.LogMethod(arena_type);
            Debug.Indent++;
            // Stop player
            PlayerController.Instance.RemoveTargetCharacter();
            // Show UI animation
            AnimationView.Visible = true;
            yield return AnimationView.AnimateDefaultBattleOpening();
            // Create arena
            var arena = ArenaController.Instance.SetArena(arena_type);
            // Create creatures
            var player_creature = CreatePlayerCreature(CharacterType.Frog);
            var opponent_creature = CreateOpponentCreature(CharacterType.Frog);
            // Move camera
            var camera_follow = CameraController.Instance.Camera.GetNodeInChildren<TopDownCameraFollow>();
            camera_follow.SetTarget(player_creature);
            // Fade to arena
            AnimationView.Background.Color = Colors.Transparent;
            AnimationView.Label.Visible = false;
            AnimationView.Visible = false;
            // Begin battle
            Debug.Indent--;

            // LOCAL HELPER METHODS
            Character CreatePlayerCreature(CharacterType type)
            {
                var creature = CharacterController.Instance.CreateCharacter(type);
                creature.GlobalPosition = arena.PlayerStart.GlobalPosition;
                PlayerController.Instance.SetTargetCharacter(creature);
                return creature;
            }

            Character CreateOpponentCreature(CharacterType type)
            {
                var creature = CharacterController.Instance.CreateCharacter(type);
                creature.GlobalPosition = arena.OpponentStart.GlobalPosition;
                return creature;
            }
        }
    }
}
