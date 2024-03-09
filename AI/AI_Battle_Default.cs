using Godot;
using System.Collections;
using System.Linq;

public partial class AI_Battle_Default : AI_Battle
{
    private enum State { Approach, Hunt, Idle }
    private State state;
    private Coroutine cr_state;

    private BattleArgs battle;
    private CreatureCharacter creature;
    private CreatureCharacter target;

    private MoveType previous_move_type;

    private Vector3 TargetPosition => target.GlobalPosition;
    private float TargetDistance => creature.GlobalPosition.DistanceTo(TargetPosition);

    public AI_Battle_Default(BattleArgs battle) : base(battle.Arena)
    {
        this.battle = battle;
    }

    public override void Initialize(Character character)
    {
        base.Initialize(character);

        creature = character as CreatureCharacter;
        creature.Health.OnDeath += OnDeath;
    }

    public override void Start()
    {
        base.Start();
        SetState(State.Idle);
        FindTarget();
    }

    public override void Stop()
    {
        base.Stop();
        StopStateCoroutine();
    }

    private void FindTarget()
    {
        Debug.TraceMethod();
        Debug.Indent++;

        if (target != null)
        {
            target.Health.OnDeath -= OnTargetDeath;
            target = null;
        }

        var creatures = creature.Team == TeamType.Opponent ? battle.PlayerCreatures.ToList() : battle.OpponentCreatures.ToList();
        var closest = creatures
            .Where(c => c.IsAlive)
            .OrderBy(c => creature.GlobalPosition.DistanceTo(c.GlobalPosition))
            .FirstOrDefault();

        target = closest;
        if (target != null)
        {
            target.Health.OnDeath += OnTargetDeath;
        }
        else
        {
            Debug.Log("Found no target");
        }

        Debug.Indent--;
    }

    private void LookAtTarget()
    {
        if (target != null)
        {
            Character.Movement.RotateTowards(target.GlobalPosition);
        }
    }

    private void SelectMove()
    {
        CreatureMove move = null;
        var valid_moves = creature.Moves.Moves.Where(x => !x.IsOnCooldown);
        var rnd = new RandomNumberGenerator();

        // Try select melee move
        if (move == null && TargetDistance < 5f)
        {
            move = valid_moves.FirstOrDefault(x => x.Info.AnimationType == MoveAnimationType.Melee);
        }

        // Try select projectile move
        if (move == null && TargetDistance > 5f)
        {
            move = valid_moves.FirstOrDefault(x => x.Info.AnimationType == MoveAnimationType.Projectile);
        }

        // Try to skip if selected same move as previous
        if (move != null && move.Info.Type == previous_move_type && rnd.RandfRange(0, 1) < 0.5f)
        {
            move = null;
        }

        // Try select any valid move
        if (move == null)
        {
            move = valid_moves.ToList().Random();
        }

        // Select move
        if (move != null)
        {
            creature.Moves.SelectMove(move);
            previous_move_type = move.Info.Type;
        }
    }

    private void OnTargetDeath()
    {
        FindTarget();
        SetState(State.Idle);
    }

    private void SetState(State state)
    {
        this.state = state;

        StopStateCoroutine();

        cr_state = state switch
        {
            State.Approach => Approach(),
            State.Hunt => Hunt(),
            State.Idle => Idle(),
            _ => null
        };

        if (cr_state == null)
        {
            Debug.LogError($"Unhandled state: {state}");
        }
    }

    private void StopStateCoroutine()
    {
        if (cr_state != null)
        {
            Coroutine.Stop(cr_state);
            cr_state = null;
        }
    }

    private void OnDeath()
    {
        Navigation.NavigationLock.AddLock("Dead");
        StopStateCoroutine();
    }

    private Coroutine Approach()
    {
        return Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            if (target.IsDead)
            {
                SetState(State.Idle);
            }

            var max_dist = 4f;
            var target_position = TargetPosition;
            Navigation.NavigatoTo(target_position);

            while (true)
            {
                var target_dist_to_position = TargetPosition.DistanceTo(target_position);
                if (target_dist_to_position > max_dist)
                {
                    target_position = TargetPosition;
                    Navigation.NavigatoTo(target_position);
                }

                var target_dist = Position.DistanceTo(TargetPosition);
                if (target_dist < max_dist)
                {
                    SetState(State.Hunt);
                }

                yield return null;
            }
        }
    }

    private Coroutine Hunt()
    {
        return Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            Navigation.NavigationLock.AddLock(state.ToString());
            while (true)
            {
                var target_dir = Position.DirectionTo(TargetPosition);
                Character.Movement.InputMove(target_dir);

                var target_dist = Position.DistanceTo(TargetPosition);
                if (target_dist < 1f)
                {
                    Navigation.NavigationLock.RemoveLock(state.ToString());
                    creature.Moves.UseSelectedMove();
                    SetState(State.Idle);
                }

                yield return null;
            }
        }
    }

    private Coroutine Idle()
    {
        return Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var rnd = new RandomNumberGenerator();
            var count = rnd.RandiRange(2, 5);
            for (int i = 0; i < count; i++)
            {
                NavigateToRandomPositionInArena();
                while (!NavigationFinished)
                {
                    yield return null;
                }

                var time = CurrentTime + rnd.RandfRange(0.2f, 1f);
                while (CurrentTime < time)
                {
                    LookAtTarget();
                    yield return null;
                }
            }

            SelectMove();
            if (creature.Moves.SelectedMove.Info.AnimationType == MoveAnimationType.Melee)
            {
                SetState(State.Approach);
            }
            else
            {
                creature.Moves.UseSelectedMove();
                SetState(State.Idle);
            }
        }
    }
}
