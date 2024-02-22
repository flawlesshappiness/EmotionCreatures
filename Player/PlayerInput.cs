using Godot;
using System.Collections.Generic;

public partial class PlayerInput : Node
{
    public readonly MultiLock MouseVisibleLock = new MultiLock();
    public readonly MultiLock InputLock = new MultiLock();

    public static PlayerInput Instance { get; private set; }

    private bool init;

    public readonly InputAction Attack = new InputAction(PlayerControls.Attack);
    public readonly InputAction Pause = new InputAction(PlayerControls.Pause);
    public readonly InputAction Submit = new InputAction(PlayerControls.Submit);
    public readonly InputAction Cancel = new InputAction(PlayerControls.Cancel);
    public readonly InputAction Forward = new InputAction(PlayerControls.Forward);
    public readonly InputAction Back = new InputAction(PlayerControls.Back);
    public readonly InputAction Left = new InputAction(PlayerControls.Left);
    public readonly InputAction Right = new InputAction(PlayerControls.Right);
    public readonly InputAction MoveNorth = new InputAction(PlayerControls.MoveNorth);
    public readonly InputAction MoveEast = new InputAction(PlayerControls.MoveEast);
    public readonly InputAction MoveSouth = new InputAction(PlayerControls.MoveSouth);
    public readonly InputAction MoveWest = new InputAction(PlayerControls.MoveWest);
    public readonly InputAction ToggleAI = new InputAction(PlayerControls.ToggleAI);
    public readonly InputDirection MoveDirection = new InputDirection(PlayerControls.Left, PlayerControls.Right, PlayerControls.Forward, PlayerControls.Back);
    public readonly InputDirection LookDirection = new InputDirection(PlayerControls.LookLeft, PlayerControls.LookRight, PlayerControls.LookForward, PlayerControls.LookBack);

    private List<InputAction> input_actions = new();
    private List<InputDirection> input_directions = new();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!init)
        {
            Initialize();
            init = true;
        }

        ProcessInputDirections();
    }

    public override void _Input(InputEvent @event)
    {
        if (InputLock.IsLocked) return;
        base._Input(@event);
        input_actions.ForEach(x => x.ProcessInput(@event));
    }

    private void ProcessInputDirections()
    {
        if (InputLock.IsLocked) return;
        input_directions.ForEach(x => x.ProcessInput());
    }

    private void Initialize()
    {
        InitializeMouse();
        InitializeInputActions();
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
        input_actions.Add(Pause);
        input_actions.Add(Submit);
        input_actions.Add(Cancel);
        input_actions.Add(Forward);
        input_actions.Add(Back);
        input_actions.Add(Left);
        input_actions.Add(Right);
        input_actions.Add(MoveNorth);
        input_actions.Add(MoveEast);
        input_actions.Add(MoveSouth);
        input_actions.Add(MoveWest);
        input_actions.Add(ToggleAI);

        input_directions.Add(MoveDirection);
        input_directions.Add(LookDirection);
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
