using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class BattleController : Node
{
    public static BattleController Instance => Singleton.TryGet<BattleController>(out var instance) ? instance : Create();

    public static BattleController Create() =>
        Singleton.Create<BattleController>($"Battle/{nameof(BattleController)}");

    private BattleAnimationView AnimationView { get; set; }

    public Action OnBattleStart, OnBattleEnd;

    private List<CreatureCharacter> OpponentCreatures { get; set; } = new();
    private List<CreatureCharacter> PlayerCreatures { get; set; } = new();
    private List<Node> BattleObjects { get; set; } = new();

    public override void _Ready()
    {
        base._Ready();

        AnimationView = View.LoadSingleton<BattleAnimationView>();
    }

    public void Clear()
    {
        OpponentCreatures.Clear();
        PlayerCreatures.Clear();

        foreach (var obj in BattleObjects.ToList())
        {
            obj.QueueFree();
        }

        BattleObjects.Clear();
    }

    public void StartBattle(ArenaType arena_type)
    {
        Clear();
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
            var arena = CreateArena();
            // Create creatures
            CreateCreatures();
            // Move camera
            var camera_follow = CameraController.Instance.Camera.GetNodeInChildren<TopDownCameraFollow>();
            camera_follow.SetTarget(PlayerCreatures.First());
            // Fade to arena
            AnimationView.Background.Color = Colors.Transparent;
            AnimationView.Label.Visible = false;
            AnimationView.Visible = false;
            // Begin battle
            OnBattleStart?.Invoke();
            Debug.Indent--;

            // LOCAL HELPER METHODS
            ArenaScene CreateArena()
            {
                return ArenaController.Instance.SetArena(arena_type);
            }

            void CreateCreatures()
            {
                var debug_player_data = new CreatureData { CharacterType = CharacterType.Frog }; // TODO
                var debug_opponent_data = new CreatureData { CharacterType = CharacterType.Frog }; // TODO
                var player_creature = CreatePlayerCreature(debug_player_data);
                var opponent_creature = CreateOpponentCreature(debug_opponent_data);
            }

            CreatureCharacter CreatePlayerCreature(CreatureData data)
            {
                var creature = CreatureController.Instance.CreateCreature(data);
                creature.GlobalPosition = arena.PlayerStart.GlobalPosition;
                PlayerController.Instance.SetTargetCharacter(creature);
                creature.Health.OnDeath += () => OnPlayerCreatureDeath(creature);

                PlayerCreatures.Add(creature);
                BattleObjects.Add(creature);
                return creature;
            }

            CreatureCharacter CreateOpponentCreature(CreatureData data)
            {
                var creature = CreatureController.Instance.CreateCreature(data);
                creature.GlobalPosition = arena.OpponentStart.GlobalPosition;
                creature.SetAI(new AI_Opponent_MVP(arena));
                creature.Health.OnDeath += () => OnOpponentCreatureDeath(creature);

                OpponentCreatures.Add(creature);
                BattleObjects.Add(creature);
                return creature;
            }
        }
    }

    private void EndBattle()
    {
        OpponentCreatures.ForEach(x => x.AI?.Stop());
        PlayerCreatures.ForEach(x => x.AI?.Stop());

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            AnimationView.Visible = true;
            yield return AnimationView.AnimateBackgroundFade(true, 1f);
            OnBattleEnd?.Invoke();
            yield return AnimationView.AnimateBackgroundFade(false, 1.0f);
            AnimationView.Visible = false;
        }
    }

    private void OnOpponentCreatureDeath(CreatureCharacter creature)
    {
        var all_dead = OpponentCreatures.All(x => x.IsDead);
        if (!all_dead) return;

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(1.0f);
            EndBattle();
        }
    }

    private void OnPlayerCreatureDeath(CreatureCharacter creature)
    {
        var all_dead = PlayerCreatures.All(x => x.IsDead);
        if (!all_dead) return;

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(1.0f);
            EndBattle();
        }
    }
}
