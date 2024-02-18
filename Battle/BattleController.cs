using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class BattleController : Node
{
    public static BattleController Instance => Singleton.TryGet<BattleController>(out var instance) ? instance : Create();
    public static BattleController Create() => Singleton.Create<BattleController>($"Battle/{nameof(BattleController)}");

    private BattleAnimationView AnimationView { get; set; }
    private BattleView BattleView { get; set; }

    public Action<StartBattleArgs> OnBattleStart;
    public Action<EndBattleArgs> OnBattleEnd;

    private List<Node> BattleObjects { get; set; } = new();

    public BattleArgs BattleArgs { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        AnimationView = View.LoadSingleton<BattleAnimationView>();
        BattleView = View.LoadSingleton<BattleView>();
    }

    public void Clear()
    {
        foreach (var obj in BattleObjects.ToList())
        {
            obj.QueueFree();
        }

        BattleObjects.Clear();

        BattleArgs = null;
    }

    public void StartBattle(StartBattleArgs args)
    {
        Clear();
        BattleArgs = new BattleArgs();

        args.PlayerTeam = Save.Game.Team; // TODO: Pick from menu after transition

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            Debug.LogMethod(args.ArenaType);
            Debug.Indent++;

            PlayerController.Instance.RemoveTargetCharacter();
            AnimationView.Visible = true;
            yield return AnimationView.AnimateDefaultBattleOpening();
            BattleArgs.Arena = CreateArena();
            CreateCreatures();

            // Move camera
            var camera_follow = CameraController.Instance.Camera.GetNodeInChildren<TopDownCameraFollow>();
            camera_follow.SetTarget(BattleArgs.PlayerCreatures.First());

            // Fade to arena
            AnimationView.Background.Color = Colors.Transparent;
            AnimationView.Label.Visible = false;
            AnimationView.Visible = false;

            // Begin battle
            BattleView.Show();
            OnBattleStart?.Invoke(args);
            Debug.Indent--;

            // LOCAL HELPER METHODS
            ArenaScene CreateArena()
            {
                return ArenaController.Instance.SetArena(args.ArenaType);
            }

            void CreateCreatures()
            {
                foreach (var data in args.PlayerTeam.Creatures)
                {
                    var creature = CreatePlayerCreature(data);
                }

                foreach (var data in args.OpponentTeam.Creatures)
                {
                    var creature = CreateOpponentCreature(data);
                }
            }

            CreatureCharacter CreatePlayerCreature(CreatureData data)
            {
                var creature = CreateCreature(data);
                creature.GlobalPosition = BattleArgs.Arena.PlayerStart.GlobalPosition;
                PlayerController.Instance.SetTargetCharacter(creature);
                creature.Health.OnDeath += () => OnPlayerCreatureDeath(creature);

                BattleArgs.PlayerCreatures.Add(creature);
                BattleObjects.Add(creature);
                return creature;
            }

            CreatureCharacter CreateOpponentCreature(CreatureData data)
            {
                var creature = CreateCreature(data);
                creature.GlobalPosition = BattleArgs.Arena.OpponentStart.GlobalPosition;
                creature.SetAI(new AI_Opponent_MVP(BattleArgs, true));
                creature.Health.OnDeath += () => OnOpponentCreatureDeath(creature);

                BattleArgs.OpponentCreatures.Add(creature);
                BattleObjects.Add(creature);
                return creature;
            }

            CreatureCharacter CreateCreature(CreatureData data)
            {
                var creature = CreatureController.Instance.CreateCreature(data);
                creature.PrepareForBattle();
                return creature;
            }
        }
    }

    private void EndBattle()
    {
        var args = new EndBattleArgs
        {

        };

        BattleArgs.PlayerCreatures.ForEach(x => x.AI?.Stop());
        BattleArgs.OpponentCreatures.ForEach(x => x.AI?.Stop());

        BattleView.Hide();

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            AnimationView.Visible = true;
            yield return AnimationView.AnimateBackgroundFade(true, 1f);
            OnBattleEnd?.Invoke(args);
            yield return AnimationView.AnimateBackgroundFade(false, 1.0f);
            AnimationView.Visible = false;
        }
    }

    private void OnOpponentCreatureDeath(CreatureCharacter creature)
    {
        var all_dead = BattleArgs.OpponentCreatures.All(x => x.IsDead);
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
        var all_dead = BattleArgs.PlayerCreatures.All(x => x.IsDead);
        if (!all_dead) return;

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(1.0f);
            EndBattle();
        }
    }
}

public class StartBattleArgs
{
    public ArenaType ArenaType { get; set; }
    public TeamData PlayerTeam { get; set; }
    public TeamData OpponentTeam { get; set; }
}

public class EndBattleArgs
{
}

public class BattleArgs
{
    public ArenaScene Arena { get; set; }
    public List<CreatureCharacter> OpponentCreatures { get; set; } = new();
    public List<CreatureCharacter> PlayerCreatures { get; set; } = new();
}