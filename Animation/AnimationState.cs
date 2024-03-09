public class AnimationState
{
    public string Animation { get; private set; } = string.Empty;
    public bool Looping { get; set; }

    public AnimationState(string animation)
    {
        Animation = animation;
    }
}
