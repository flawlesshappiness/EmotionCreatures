using System;

public partial class Health
{
    private float value, max;
    private bool dead;

    public Action OnValueChanged;
    public Action OnDeath;

    public float Value => value;
    public float Max => max;
    public bool IsDead => dead;

    public Health(float max)
    {
        this.max = max;
        value = max;
    }

    public void SetValue(float value)
    {
        this.value = Math.Clamp(value, 0, max);

        if (value == 0 && !dead)
        {
            dead = true;
            OnDeath?.Invoke();
        }

        OnValueChanged?.Invoke();
    }

    public void AdjustValue(float adjust)
    {
        SetValue(value + adjust);
    }

    public void Kill()
    {
        SetValue(0);
    }
}
