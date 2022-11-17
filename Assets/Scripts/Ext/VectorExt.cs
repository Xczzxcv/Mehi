using UnityEngine;

namespace Ext
{
public static class VectorExt
{
    public static Vector3 ToV3(this Vector2Int v2Int)
    {
        return new Vector3(v2Int.x, v2Int.y, 0);
    }

    public static Vector2Int ToV2I(this Vector3 v3)
    {
        return new Vector2Int(Mathf.RoundToInt(v3.x), Mathf.RoundToInt(v3.y));
    }
}
}