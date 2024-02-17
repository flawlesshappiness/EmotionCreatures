using Godot;

public partial class GameMenuView : View
{
    [NodeType(typeof(MenuOptionsContainer))]
    public MenuOptionsContainer Options;

    [NodeName(nameof(SFXOpen))]
    public AudioStreamPlayer SFXOpen;

    [NodeName(nameof(SFXClose))]
    public AudioStreamPlayer SFXClose;

    public override void _Ready()
    {
        base._Ready();

        AddOptions();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        PressPause();
    }

    protected override void OnShow()
    {
        base.OnShow();
        PlayerInput.Instance.MouseVisibleLock.AddLock(nameof(GameMenuView));
        PlayerInput.Instance.InputLock.AddLock(nameof(GameMenuView));
        Options.GrabFocus();
        SFXOpen.Play();
        Game.OnMenuOpen?.Invoke();
    }

    protected override void OnHide()
    {
        base.OnHide();
        PlayerInput.Instance.MouseVisibleLock.RemoveLock(nameof(GameMenuView));
        PlayerInput.Instance.InputLock.RemoveLock(nameof(GameMenuView));
        SFXClose.Play();
        Game.OnMenuClose?.Invoke();
    }

    private void PressPause()
    {
        if (Input.IsActionJustPressed(PlayerControls.Pause))
        {
            if (IsVisibleInTree())
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    private void AddOptions()
    {
        Options.CreateOption("Team", OpenTeam);
        Options.CreateOption("Quit game", Game.Quit);
        Options.CreateOption("Close menu", Hide);
    }

    private void OpenTeam()
    {
        Hide();
        Show<TeamView>();
    }
}
