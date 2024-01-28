public class Player
{
    public readonly MultiLock InteractLock = new MultiLock();

    public static Player Instance { get; set; }

    public static void SaveData()
    {
        if (Instance != null)
        {
            //Instance.SaveData();
        }
    }

    public static void LoadData()
    {
        if (Instance != null)
        {
            //Instance.LoadData();
        }
    }
}
