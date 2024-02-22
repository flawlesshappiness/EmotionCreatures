using Godot;
using System;

public static class Lerp
{
    private static T _Lerp<T>(T v1, T v2, float t, Func<float, T, T> mul_func, Func<T, T, T> add_func) =>
        add_func(mul_func(1 - t, v1), mul_func(t, v2));

    public static float Float(float v1, float v2, float t) =>
        _Lerp(v1, v2, t, (a, b) => a * b, (a, b) => a + b);

    public static Vector3 Vector(Vector3 v1, Vector3 v2, float t) =>
        _Lerp(v1, v2, t, (a, b) => a * b, (a, b) => a + b);

    public static Vector2 Vector(Vector2 v1, Vector2 v2, float t) =>
        _Lerp(v1, v2, t, (a, b) => a * b, (a, b) => a + b);

    public static Transform3D Transform(Transform3D v1, Transform3D v2, float t) =>
        v1.InterpolateWith(v2, t);

    public static Transform2D Transform(Transform2D v1, Transform2D v2, float t) =>
        v1.InterpolateWith(v2, t);

    public static Variant Variant(Variant v1, Variant v2, float t)
    {
        if (v1.VariantType != v2.VariantType)
            throw new ArgumentException($"Variant type {v1.VariantType} does not match variant type {v2.VariantType}");

        switch (v1.VariantType)
        {
            case Godot.Variant.Type.Float:
                return Float(((float)v1), ((float)v2), t);

            case Godot.Variant.Type.Vector2:
                return Vector(v1.AsVector2(), v2.AsVector2(), t);

            case Godot.Variant.Type.Vector3:
                return Vector(v1.AsVector3(), v2.AsVector3(), t);

            case Godot.Variant.Type.Transform2D:
                return Transform(v1.AsTransform2D(), v2.AsTransform2D(), t);

            case Godot.Variant.Type.Transform3D:
                return Transform(v1.AsTransform3D(), v2.AsTransform3D(), t);

            default:
                throw new ArgumentException($"Failed to Lerp variant of type: {v1.VariantType}");
        }
    }
}
