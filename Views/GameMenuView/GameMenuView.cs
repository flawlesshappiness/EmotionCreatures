using Godot;

public partial class GameMenuView : View
{
    [NodeType(typeof(MenuOptionsContainer))]
    public MenuOptionsContainer Options;

    public override void _Ready()
    {
        base._Ready();

        AddOptions();
        CloseMenu();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        PressPause();
    }

    private void PressPause()
    {
        if (Input.IsActionJustPressed(PlayerControls.Pause))
        {
            OpenMenu();
        }
    }

    private void AddOptions()
    {
        Options.CreateOption("Quit game", Game.Quit);
        Options.CreateOption("Close menu", CloseMenu);
    }

    private void OpenMenu()
    {
        Debug.TraceMethod();
        if (!Options.HasSelected)
        {
            Options.SetSelected(0);
        }

        PlayerInput.Instance.MouseVisibleLock.AddLock(nameof(GameMenuView));
        Show();
        Game.OnMenuOpen?.Invoke();
    }

    private void CloseMenu()
    {
        Debug.TraceMethod();
        PlayerInput.Instance.MouseVisibleLock.RemoveLock(nameof(GameMenuView));
        Hide();
        Game.OnMenuClose?.Invoke();
    }
}
