using System.Collections.Generic;

public class GameSaveData : SaveData
{
    public string Scene { get; set; } = "MVP";

    public List<SceneData> Scenes { get; set; } = new();
}
