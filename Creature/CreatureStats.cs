using Godot;

public class CreatureStats
{
    public const int MIN_LEVEL = 1;
    public const int MAX_LEVEL = 100;

    public int Level { get; private set; }
    public float Health { get; private set; }

    public static CreatureStats FromLevel(CreatureInfo info, int level)
    {
        var stats = new CreatureStats();

        var t = ((float)level - MIN_LEVEL) / (MAX_LEVEL - MIN_LEVEL);

        stats.Level = level;
        stats.Health = Mathf.Lerp(info.HealthMin, info.HealthMax, t);

        return stats;
    }

    public void Trace()
    {
        Debug.Trace("CreatureStats:");
        Debug.Indent++;
        Debug.Trace($"Level: {Level}");
        Debug.Trace($"Health: {Health}");
        Debug.Indent--;
    }
}
