public interface IDataListItem<T>
{
    string Id { get; set; }

    void Save(T reference);

    void Load();
}
