using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ParticleSystemFunc
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetSampleTexture(this ParticleSystem particleSystem, Sprite sprite)
    {
        var shapeModule = particleSystem.shape;
        shapeModule.texture = sprite?.texture;
    }
}
