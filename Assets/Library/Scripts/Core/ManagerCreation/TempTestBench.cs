using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class TempTestBench : MonoBehaviour
{
    [Button(nameof(AngleToDirection))]
    public Vector2 AngleToDirection(float angle)
    {
        return angle.ClockwiseAngleToDirection();
    }

    [Button(nameof(SignedAngle))]
    public float SignedAngle(Vector2 a, Vector2 b)
    {
        return a.SignedAngle(b);
    }

    [Button(nameof(ClockwiseAngle))]
    public float ClockwiseAngle(Vector2 a, Vector2 b)
    {
        return a.ClockwiseAngle(b);
    }
}
