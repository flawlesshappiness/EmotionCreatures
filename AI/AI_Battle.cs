public partial class AI_Battle : AI
{
    protected ArenaScene Arena { get; private set; }

    public AI_Battle(ArenaScene arena)
    {
        Arena = arena;
    }

    protected void NavigateToRandomPositionInArena()
    {
        Navigation.NavigatoToRandomPosition(Arena.World.GlobalPosition, 5, 2);
    }
}
