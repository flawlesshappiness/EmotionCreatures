using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class BattleController : SingletonController<BattleController>
{
    public static BattleController Instance => GetController("Battle");

    private BattleAnimationView AnimationView => View.Get<BattleAnimationView>();
    private BattleView BattleView => View.Get<BattleView>();
    private CreatureSelectView CreatureSelectView => View.Get<CreatureSelectView>();

    public Action<StartBattleArgs> OnBattleStart;
    public Action<EndBattleArgs> OnBattleEnd;
    public Action OnToggleAI;
    public Action<CreatureCharacter> OnPlayerTargetCreatureChanged;

    public BattleArgs BattleArgs { get; private set; }
    public bool HasActiveBattle => BattleArgs != null;

    public CreatureCharacter TargetPlayerCreature { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        PlayerInput.Instance.ToggleAI.OnPressed += TogglePlayerAI;
        PlayerInput.Instance.MoveDirection.OnStarted += ChangePlayerTarget;
    }

    public void Clear()
    {
        if (BattleArgs != null)
        {
            if (BattleArgs.Arena != null)
            {
                BattleArgs.Arena.QueueFree();
            }

            foreach (var obj in BattleArgs.BattleObjects.ToList())
            {
                obj.QueueFree();
            }
        }

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
            PlayerController.Instance.SetTargetCharacter(TargetPlayerCreature);
            CameraBrain.MainCamera.TeleportTo(TargetPlayerCreature.VCam);

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
                var rnd = new RandomNumberGenerator();
                var x = rnd.RandfRange(-0.1f, 0.1f);
                var z = rnd.RandfRange(-0.1f, 0.1f);
                var offset = new Vector3(x, 0, z);

                var creature = CreateCreature(data, TeamType.Player);
                creature.GlobalPosition = BattleArgs.Arena.PlayerStart.GlobalPosition + offset;
                creature.Health.OnDeath += () => OnPlayerCreatureDeath(creature);
                creature.SetAI(new AI_Battle_Default(BattleArgs));
                creature.HealthBar.SetBlue();

                BattleArgs.PlayerCreatures.Add(creature);
                BattleArgs.BattleObjects.Add(creature);
                return creature;
            }

            CreatureCharacter CreateOpponentCreature(CreatureData data)
            {
                var rnd = new RandomNumberGenerator();
                var x = rnd.RandfRange(-0.1f, 0.1f);
                var z = rnd.RandfRange(-0.1f, 0.1f);
                var offset = new Vector3(x, 0, z);

                var creature = CreateCreature(data, TeamType.Opponent);
                creature.GlobalPosition = BattleArgs.Arena.OpponentStart.GlobalPosition + offset;
                creature.Health.OnDeath += () => OnOpponentCreatureDeath(creature);
                creature.SetAI(new AI_Battle_Default(BattleArgs));
                creature.HealthBar.SetRed();

                BattleArgs.OpponentCreatures.Add(creature);
                BattleArgs.BattleObjects.Add(creature);
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

        Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            AnimationView.Clear();
            AnimationView.Show();
            yield return AnimationView.AnimateBackgroundFade(true, 1f);
            BattleView.Hide();
            Clear();
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
        TargetPlayerCreature = creature;
        OnPlayerTargetCreatureChanged?.Invoke(TargetPlayerCreature);
    }

    private void ChangePlayerTarget(Vector2 input)
    {
        if (!HasActiveBattle) return;
        if (TargetPlayerCreature == null) return;
        if (!TargetPlayerCreature.AI.Active && TargetPlayerCreature.IsAlive) return;

        var direction = CameraBrain.MainCamera.Basis * new Vector3(input.X, 0, input.Y).Normalized();
        var position = TargetPlayerCreature.GlobalPosition;
        var best_dot = -1f;
        var best_creature = TargetPlayerCreature;

        foreach (var creature in BattleArgs.PlayerCreatures)
        {
            var dir_to_creature = (creature.GlobalPosition - position).Normalized();
            var dot = direction.Dot(dir_to_creature);
            var better = dot > best_dot;
            best_dot = better ? dot : best_dot;
            best_creature = better ? creature : best_creature;
        }

        if (best_dot > 0)
        {
            SetPlayerTarget(best_creature);
            CameraBrain.MainCamera.MoveTo(best_creature.VCam, 0.5f, Curves.EaseInOutQuad);
        }
    }

    private void TogglePlayerAI()
    {
        if (!HasActiveBattle) return;
        if (TargetPlayerCreature.IsDead) return;

        TargetPlayerCreature.AI.Toggle();

        if (!TargetPlayerCreature.AI.Active)
        {
            PlayerController.Instance.SetTargetCharacter(TargetPlayerCreature);
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
    public List<Node> BattleObjects { get; set; } = new();

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