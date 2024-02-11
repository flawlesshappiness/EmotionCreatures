using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInput : Node
{
    public readonly MultiLock MouseVisibleLock = new MultiLock();

    public static PlayerInput Instance { get; private set; }

    private Character Target => PlayerController.Instance.TargetCharacter;

    private bool HasTarget => Target != null;

    public readonly InputAction Attack = new InputAction(PlayerControls.Attack);

    private List<InputAction> input_actions = new();

    public override void _Ready()
    {
        base._Ready();

        InitializeMouse();
        InitializeInputActions();

        Instance = this;
    }

    private void InitializeMouse()
    {
        MouseVisibleLock.OnLocked += OnMouseVisibleLocked;
        MouseVisibleLock.OnFree += OnMouseVisibleFree;

        if (MouseVisibleLock.IsLocked)
        {
            OnMouseVisibleLocked();
        }
        else
        {
            OnMouseVisibleFree();
        }
    }

    private void InitializeInputActions()
    {
        input_actions.Add(Attack);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var fdelta = Convert.ToSingle(delta);
        ProcessInputTargetCharacter(fdelta);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        input_actions.ForEach(x => x.ProcessInput(@event));
    }

    private void ProcessInputTargetCharacter(float delta)
    {
        if (!HasTarget) return;
        var input = Input.GetVector(PlayerControls.Left, PlayerControls.Right, PlayerControls.Forward, PlayerControls.Back);
        Target.Movement.Move(input, delta);
    }

    public PlayerInput()
    {
        MouseVisibleLock.OnLocked += OnMouseVisibleLocked;
        MouseVisibleLock.OnFree += OnMouseVisibleFree;
    }

    private void OnMouseVisibleFree()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private void OnMouseVisibleLocked()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
}
