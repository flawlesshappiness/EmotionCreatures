using Godot;
using System;

public class AI
{
    protected Character Character { get; private set; }

    protected CharacterNavigation Navigation => Character.Navigation;
    protected bool NavigationFinished => Navigation.IsNavigationFinished();
    protected double CurrentTime => Time.GetUnixTimeFromSystem();
    protected Vector3 Position => Character.GlobalPosition;

    public bool Active { get; private set; }

    public Action<bool> OnActiveChanged;

    public virtual void Initialize(Character character)
    {
        Character = character;
    }

    public virtual void Process()
    {

    }

    public bool Toggle()
    {
        if (Active)
        {
            Stop();
        }
        else
        {
            Start();
        }

        Debug.TraceMethod(Active);
        return Active;
    }

    public bool TryStart()
    {
        if (!Active)
        {
            Start();
            return true;
        }

        return false;
    }

    public bool TryStop()
    {
        if (Active)
        {
            Stop();
            return true;
        }

        return false;
    }

    public virtual void Start()
    {
        Active = true;
        Navigation.NavigationLock.RemoveLock(nameof(AI));
        OnActiveChanged?.Invoke(Active);
    }

    public virtual void Stop()
    {
        Active = false;
        Navigation.NavigationLock.AddLock(nameof(AI));
        Character.Movement.Stop();
        OnActiveChanged?.Invoke(Active);
    }
}
