using Godot;
using System;

public static class Lerp
{
    private static T _Lerp<T>(T v1, T v2, float t, Func<float, T, T> mul_func, Func<T, T, T> add_func) =>
        add_func(mul_func(1 - t, v1), mul_func(t, v2));

    public static float Float(float v1, float v2, float t) =>
        _Lerp(v1, v2, t, (a, b) => a * b, (a, b) => a + b);

    public static Vector3 Vector3(Vector3 v1, Vector3 v2, float t) =>
        _Lerp(v1, v2, t, (a, b) => a * b, (a, b) => a + b);

    public static Vector2 Vector2(Vector2 v1, Vector2 v2, float t) =>
        _Lerp(v1, v2, t, (a, b) => a * b, (a, b) => a + b);

    public static Variant Variant(Variant v1, Variant v2, float t)
    {
        if (v1.VariantType != v2.VariantType)
            throw new ArgumentException($"Variant type {v1.VariantType} does not match variant type {v2.VariantType}");

        switch (v1.VariantType)
        {
            case Godot.Variant.Type.Float:
                return Float(((float)v1), ((float)v2), t);

            case Godot.Variant.Type.Vector2:
                return Vector2(v1.AsVector2(), v2.AsVector2(), t);

            case Godot.Variant.Type.Vector3:
                return Vector3(v1.AsVector3(), v2.AsVector3(), t);

            default:
                throw new ArgumentException($"Failed to Lerp variant of type: {v1.VariantType}");
        }
    }
}
