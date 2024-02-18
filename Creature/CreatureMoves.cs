using Godot;
using System;
using System.Collections.Generic;

public partial class CreatureMoves : Node
{
    public List<CreatureMove> Moves = new();

    public CreatureMove SelectedMove;

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
            Debug.LogError("Could not select move: Move was null");
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

    public bool TryUseSelectedMove()
    {
        if (SelectedMove == null) return false;
        if (SelectedMove.IsOnCooldown) return false;

        SelectedMove.Use();
        return true;
    }
}

public class CreatureMove
{
    public MoveInfo Info { get; set; }
    public CreatureCharacter Creature { get; set; }
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
