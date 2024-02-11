using Godot;

public class AI
{
    protected Character Character { get; private set; }

    protected CharacterNavigation Navigation => Character.Navigation;
    protected bool NavigationFinished => Navigation.IsNavigationFinished();
    protected double CurrentTime => Time.GetUnixTimeFromSystem();
    protected Vector3 Position => Character.GlobalPosition;

    public virtual void Initialize(Character character)
    {
        Character = character;
    }

    public virtual void Process()
    {

    }

    public virtual void Stop()
    {

    }
}
