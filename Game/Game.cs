using System;

public static class Game
{
    public static Action OnQuit;

    public static void Quit()
    {
        OnQuit?.Invoke();
        Debug.WriteLogsToPersistentData();
        Scene.Tree.Quit();
    }
}
