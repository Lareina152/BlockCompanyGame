using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public enum FourTypesDirection2D
{
    [LabelText("无方向")]
    None,
    [LabelText("上")]
    Up,
    [LabelText("下")]
    Down,
    [LabelText("左")]
    Left,
    [LabelText("右")]
    Right
}

public enum LeftRightDirection
{
    [LabelText("无方向")]
    None,
    [LabelText("左")]
    Left,
    [LabelText("右")]
    Right
}

public static class DirectionFunc
{
    #region Four Types Direction 2D

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FourTypesDirection2D Reversed(this FourTypesDirection2D direction)
    {
        return direction switch
        {
            FourTypesDirection2D.Up => FourTypesDirection2D.Down,
            FourTypesDirection2D.Down => FourTypesDirection2D.Up,
            FourTypesDirection2D.Left => FourTypesDirection2D.Right,
            FourTypesDirection2D.Right => FourTypesDirection2D.Left,
            _ => FourTypesDirection2D.None
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FourTypesDirection2D ConvertToFourTypesDirection2D(this Vector2 vector)
    {
        if (vector.x.Abs() > vector.y.Abs())
        {
            return vector.x > 0 ? FourTypesDirection2D.Right : FourTypesDirection2D.Left;
        }

        if (vector.y.Abs() > vector.x.Abs())
        {
            return vector.y > 0 ? FourTypesDirection2D.Up : FourTypesDirection2D.Down;
        }

        return FourTypesDirection2D.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int ConvertToCardinalVector(this FourTypesDirection2D direction)
    {
        return direction switch
        {
            FourTypesDirection2D.None => Vector2Int.zero,
            FourTypesDirection2D.Up => Vector2Int.up,
            FourTypesDirection2D.Down => Vector2Int.down,
            FourTypesDirection2D.Left => Vector2Int.left,
            FourTypesDirection2D.Right => Vector2Int.right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    #endregion

    #region Left Right Direction

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LeftRightDirection Reversed(LeftRightDirection direction)
    {
        return direction switch
        {
            LeftRightDirection.Left => LeftRightDirection.Right,
            LeftRightDirection.Right => LeftRightDirection.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    #endregion
}
