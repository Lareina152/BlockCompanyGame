using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

public static class GizmosExt
{
    //public static void DrawRect(Vector2 start, Vector2 size, float z = 0)
    //{
    //    Assert.IsAbove(size, new(0, 0));

    //    Gizmos.DrawLine(new Vector3(start.x, start.y, z),
    //        new Vector3(start.x + size.x, start.y, z));
    //    Gizmos.DrawLine(new Vector3(start.x + size.x, start.y, z),
    //        new Vector3(start.x + size.x, start.y + size.y, z));
    //    Gizmos.DrawLine(new Vector3(start.x + size.x, start.y + size.y, z),
    //        new Vector3(start.x, start.y + size.y, z));
    //    Gizmos.DrawLine(new Vector3(start.x, start.y + size.y, z),
    //        new Vector3(start.x, start.y, z));
    //}

    //public static void DrawRectFromScreenPoint(Vector2 start, Vector2 size, float z = 0)
    //{
    //    DrawRect(start.ToScreenPoint(), (start + size).ToScreenPoint());
    //}

    public static void DrawWireCube(Vector3 center, Vector3 size)
    {
        Gizmos.DrawWireCube(center, size);
    }

    public static void DrawWireCube(Vector3 center, Vector3 size, Quaternion rotation)
    {
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
