using System;

public class Health : ClampedFloat
{
    public Action OnDeath;
    public bool IsDead => IsAtMin;
    public bool IsAlive => !IsDead;

    public Health(float max) : base(0, max, max)
    {
        OnMin += () => OnDeath?.Invoke();
    }

    public void Kill() => SetValueToMin();
}
