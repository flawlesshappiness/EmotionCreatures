public abstract class SaveData
{
    public void Serialize()
    {
        var type = GetType();
        SaveDataController.Instance.Save(type);
    }
}
