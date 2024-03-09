using FlawLizArt.Animation.StateMachine;
using Godot;
using System;
using System.Collections.Generic;

public partial class CreatureMoves : Node
{
    public List<CreatureMove> Moves = new();

    public CreatureMove SelectedMove;
    public int SelectedMoveIndex => SelectedMove == null ? 0 : Moves.IndexOf(SelectedMove);
    public CreatureCharacter Creature { get; private set; }

    public Action<CreatureMove> OnMoveFinished;

    public void Initialize(CreatureCharacter creature)
    {
        Creature = creature;
    }

    public void LoadMoves(MovesetData data)
    {
        Debug.TraceMethod(data);
        Debug.Indent++;

        if (data == null)
        {
            Debug.LogError($"Failed to load moves when MovesetData is null");
            return;
        }

        Moves.Clear();
        foreach (var type in data.Moves)
        {
            var info = MoveController.Instance.GetInfo(type);
            var move = new CreatureMove
            {
                Info = info,
                Creature = Creature,
                AnimationState = GetAnimationState(info.AnimationType)
            };

            Moves.Add(move);
        }

        Debug.Indent--;
    }

    public void SelectMove(int i) => SelectMove(Moves.Get(i));

    public void SelectMove(CreatureMove move)
    {
        if (move == null)
        {
            return;
        }

        Debug.TraceMethod(move?.Info?.Type);
        Debug.Indent++;

        if (SelectedMove != null)
        {
            SelectedMove.OnDeselected?.Invoke();
            SelectedMove = null;
        }

        SelectedMove = move;
        SelectedMove.OnSelected?.Invoke();

        Debug.Indent--;
    }

    private bool TryUseMove(CreatureMove move)
    {
        if (Creature.Health.IsDead) return false;
        if (SelectedMove == null) return false;
        if (SelectedMove.IsOnCooldown) return false;
        if (IsAttacking()) return false;

        UseMove(move);
        return true;
    }

    public void UseSelectedMove()
    {
        UseMove(SelectedMove);
    }

    public bool TryUseSelectedMove()
    {
        return TryUseMove(SelectedMove);
    }

    public void UseMove(CreatureMove move)
    {
        move.Use();
        Creature.Combat.CurrentMove = move;
        Creature.CreatureAnimator.SetCurrentState(move.AnimationState);

        move.AnimationState.OnExit += MoveFinished;
        move.AnimationState.OnExit += () => move.AnimationState.OnExit -= MoveFinished;
    }

    private void MoveFinished()
    {
        OnMoveFinished?.Invoke(Creature.Combat.CurrentMove);
    }

    public bool IsAttacking()
    {
        var animator = Creature.CreatureAnimator;
        return animator.Current == animator.AttackMelee || animator.Current == animator.AttackProjectile;
    }

    private StateNode GetAnimationState(MoveAnimationType type) => type switch
    {
        MoveAnimationType.Melee => Creature.CreatureAnimator.AttackMelee,
        MoveAnimationType.Projectile => Creature.CreatureAnimator.AttackProjectile,
        _ => Creature.CreatureAnimator.AttackMelee
    };
}

public class CreatureMove
{
    public MoveInfo Info { get; set; }
    public CreatureCharacter Creature { get; set; }
    public StateNode AnimationState { get; set; }
    public float TimeCooldownStart { get; set; }
    public float TimeCooldownEnd { get; set; }
    public int TimesUsed { get; set; }

    public MoveEffect Effect => effect ?? (effect = CreateEffect());
    public bool IsOnCooldown => TimeHelper.CurrentTime < TimeCooldownEnd;

    public Action OnSelected, OnDeselected;

    private MoveEffect effect;

    public void Use()
    {
        Debug.TraceMethod(Info.Type);
        Debug.Indent++;

        TimeCooldownStart = TimeHelper.CurrentTime;
        TimeCooldownEnd = TimeCooldownStart + Info.Cooldown * 1000;
        TimesUsed++;

        Debug.Indent--;
    }

    private MoveEffect CreateEffect() => new MoveEffect
    {
        Team = Creature.Team,
        Sender = Creature,
        Damage = Info.Damage
    };
}
