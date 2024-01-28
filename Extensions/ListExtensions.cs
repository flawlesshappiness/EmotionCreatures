using Godot;
using System.Collections.Generic;

public static class ListExtensions
{
    public static T Random<T>(this List<T> list, RandomNumberGenerator rng = null)
    {
        rng ??= new RandomNumberGenerator();
        return list[rng.RandiRange(0, list.Count - 1)];
    }
}
