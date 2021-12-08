using UnityEngine;

public static class ExtensionMethods
{
    public static T AddComponentOrGetIfExists<T>(this GameObject g) where T: Component
    {
        var c = g.GetComponent<T>();
        if (c)
            return c;
        else
            return g.AddComponent<T>();
    }
    public static T Random<T>(this T[] array) where T: Object
    {
        if (array.Length > 0)
            return array[UnityEngine.Random.Range(0, array.Length)];
        else
            return null;
    }
    public static void PrintError(this GameObject gameObject, string message)
    {
        Debug.LogError(message);
    }
    public static void PrintName(this GameObject gameObject)
    {
        Debug.Log(gameObject.name);
    }
    public static Vector3 ToWorld(this Transform transform, Vector3 local)
    {
        return transform.forward * local.z + transform.right * local.x + transform.up * local.y;
    }
}
