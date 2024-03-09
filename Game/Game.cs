using System;

public static class Game
{
    public static Action OnQuit;
    public static Action OnMenuOpen, OnMenuClose;

    public static void Quit()
    {
        OnQuit?.Invoke();
        Debug.WriteLogsToPersistentData();
        Scene.Tree.Quit();
    }

    public static void Save()
    {
        SaveDataController.Instance.SaveAll();
    }

    public static void RegisterDebugActions()
    {
        var category = "GAME";

        Debug.RegisterAction(new DebugAction
        {
            Text = "Change scene",
            Category = category,
            Action = DebugChangeScene
        });
    }

    private static void DebugChangeScene(DebugView view)
    {
        view.HideContent();

        view.ContentSearch.AddItem("MVP", () => ChangeScene("MVP"));
        view.ContentSearch.AddItem("TerrainTest", () => ChangeScene("TerrainTest"));
        view.Content.Show();
        view.ContentSearch.Show();

        void ChangeScene(string scene)
        {
            Scene.Goto(scene);
        }
    }
}
