using System.Collections.Generic;

public class GameSaveData : SaveData
{
    public string Scene { get; set; } = "MVP";
    public List<SceneData> Scenes { get; set; } = new();
    public Dictionary<string, bool> DialogueFlags { get; set; } = new();
    public Dictionary<string, DialogueCharacterData> DialogueCharacters { get; set; } = new();
    public TeamData Team { get; set; } = new();
}
