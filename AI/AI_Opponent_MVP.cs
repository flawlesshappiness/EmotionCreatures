using Godot;
using System.Collections;

public class AI_Opponent_MVP : AI_Battle
{
    private enum State { Approach, Hunt, Idle }
    private State state;
    private Coroutine cr_state;

    private CreatureCharacter target;

    private Vector3 TargetPosition => target.GlobalPosition;

    private CreatureCharacter Creature;

    public AI_Opponent_MVP(ArenaScene arena) : base(arena)
    {

    }

    public override void Initialize(Character character)
    {
        base.Initialize(character);
        SetState(State.Idle);

        Creature = character as CreatureCharacter;
        Creature.Health.OnDeath += OnDeath;

        FindTarget();
    }

    public override void Stop()
    {
        base.Stop();
        StopStateCoroutine();
    }

    private void FindTarget()
    {
        if (target != null)
        {
            target.Health.OnDeath -= OnTargetDeath;
            target = null;
        }

        target = PlayerController.Instance.TargetCharacter as CreatureCharacter;

        if (target != null)
        {
            target.Health.OnDeath += OnTargetDeath;
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
                Character.Movement.Move(target_dir);

                var target_dist = Position.DistanceTo(TargetPosition);
                if (target_dist < 1f)
                {
                    Navigation.NavigationLock.RemoveLock(state.ToString());
                    Creature.Attack();
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
                    Character.Movement.RotateTowards(target.GlobalPosition);
                    yield return null;
                }
            }

            SetState(State.Approach);
        }
    }
}
