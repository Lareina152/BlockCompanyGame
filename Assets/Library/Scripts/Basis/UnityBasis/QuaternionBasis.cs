using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class QuaternionBasis
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion Lerp(this Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Lerp(a, b, t);
    }
}
