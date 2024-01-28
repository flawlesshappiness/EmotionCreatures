using System;
using System.Collections.Generic;
using System.Linq;

public class DataList<T, I> : List<T> where T : IDataListItem<I>, new()
{
    private string DebugName => GetDebugName();

    public T GetOrCreate(string id)
    {
        Debug.Log($"{DebugName}.GetOrCreate");
        Debug.Indent++;

        var item = this.FirstOrDefault(item => item.Id == id);

        if (item == null)
        {
            item = new T()
            {
                Id = id
            };

            Add(item);
        }

        Debug.Indent--;
        return item;
    }

    public void Save(I reference, Func<I, string> getId)
    {
        Debug.Log($"{DebugName}.Save");
        Debug.Indent++;

        if (reference == null)
        {
            Debug.LogError("Reference was null");
            Debug.Indent--;
            return;
        }

        var id = getId(reference);
        var data = GetOrCreate(id);
        data.Save(reference);

        Debug.Indent--;
    }

    public void Load()
    {
        Debug.Log($"{DebugName}.Load");
        Debug.Indent++;

        foreach (var data in this)
        {
            try
            {
                data.Load();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        Debug.Indent--;
    }

    private string GetDebugName()
    {
        var t_name = typeof(T).Name;
        var i_name = typeof(I).Name;
        return $"DataList<{t_name}, {i_name}>";
    }
}
