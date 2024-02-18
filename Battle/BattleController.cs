using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class BattleController : Node
{
    public static BattleController Instance => Singleton.TryGet<BattleController>(out var instance) ? instance : Create();
    public static BattleController Create() => Singleton.Create<BattleController>($"Battle/{nameof(BattleController)}");

    private BattleAnimationView AnimationView => View.Get<BattleAnimationView>();
    private BattleView BattleView => View.Get<BattleView>();
    private CreatureSelectView CreatureSelectView => View.Get<CreatureSelectView>();

    public Action<StartBattleArgs> OnBattleStart;
    public Action<EndBattleArgs> OnBattleEnd;
    public Action OnToggleAI;

    private List<Node> BattleObjects { get; set; } = new();
    public BattleArgs BattleArgs { get; private set; }
    public bool HasActiveBattle => BattleArgs != null;

    public CreatureCharacter TargetPlayerCharacter { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        PlayerInput.Instance.ToggleAI.OnPressed += TogglePlayerAI;
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

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            Debug.LogMethod(args.ArenaType);
            Debug.Indent++;

            PlayerController.Instance.RemoveTargetCharacter();
            AnimationView.Clear();
            AnimationView.Show();
            yield return AnimationView.AnimateDefaultBattleOpening();
            BattleArgs.Arena = CreateArena();

            // Select team
            AnimationView.Hide();
            CreatureSelectView.Show();
            yield return CreatureSelectView.WaitForPlayerToPickTeam(Save.Game.Team, args.OpponentTeam.Creatures.Count);
            args.PlayerTeam = CreatureSelectView.SelectedTeam;

            // Create creatures
            CreateCreatures();

            // Move camera
            SetPlayerTarget(BattleArgs.PlayerCreatures.First());
            PlayerController.Instance.SetTargetCharacter(TargetPlayerCharacter);
            var camera_follow = CameraController.Instance.Camera.GetNodeInChildren<TopDownCameraFollow>();
            camera_follow.SetTarget(TargetPlayerCharacter);

            // Begin battle
            BattleView.Show();
            BattleArgs.StartBattle();
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
                var creature = CreateCreature(data, TeamType.Player);
                creature.GlobalPosition = BattleArgs.Arena.PlayerStart.GlobalPosition;
                creature.Health.OnDeath += () => OnPlayerCreatureDeath(creature);
                creature.SetAI(new AI_Battle_Default(BattleArgs));

                BattleArgs.PlayerCreatures.Add(creature);
                BattleObjects.Add(creature);
                return creature;
            }

            CreatureCharacter CreateOpponentCreature(CreatureData data)
            {
                var creature = CreateCreature(data, TeamType.Opponent);
                creature.GlobalPosition = BattleArgs.Arena.OpponentStart.GlobalPosition;
                creature.Health.OnDeath += () => OnOpponentCreatureDeath(creature);
                creature.SetAI(new AI_Battle_Default(BattleArgs));

                BattleArgs.OpponentCreatures.Add(creature);
                BattleObjects.Add(creature);
                return creature;
            }

            CreatureCharacter CreateCreature(CreatureData data, TeamType team)
            {
                var creature = CreatureController.Instance.CreateCreature(data);
                creature.PrepareForBattle(team);
                return creature;
            }
        }
    }

    private void EndBattle()
    {
        var args = new EndBattleArgs
        {

        };

        BattleArgs.StopBattle();
        BattleArgs = null;

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            AnimationView.Clear();
            AnimationView.Show();
            yield return AnimationView.AnimateBackgroundFade(true, 1f);
            BattleView.Hide();
            OnBattleEnd?.Invoke(args);
            yield return AnimationView.AnimateBackgroundFade(false, 1.0f);
            AnimationView.Hide();
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

    private void SetPlayerTarget(CreatureCharacter creature)
    {
        TargetPlayerCharacter = creature;
    }

    private void TogglePlayerAI()
    {
        if (!HasActiveBattle) return;
        TargetPlayerCharacter.AI.Toggle();

        if (!TargetPlayerCharacter.AI.Active)
        {
            PlayerController.Instance.SetTargetCharacter(TargetPlayerCharacter);
        }

        OnToggleAI?.Invoke();
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

    public void StartBattle()
    {
        OpponentCreatures.ForEach(x => x.AI.Start());
        PlayerCreatures.ForEach(x => x.AI.Start());
    }

    public void StopBattle()
    {
        OpponentCreatures.ForEach(x => x.AI.Stop());
        PlayerCreatures.ForEach(x => x.AI.Stop());
    }
}