using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Basis
{
    #region Enum Definations

    public enum DistanceType
    {
        [LabelText("曼哈顿距离")]
        Manhattan,
        [LabelText("欧式距离")]
        Euclidean
    }

    public enum PlaneType
    {
        [LabelText("XY平面")]
        XY,
        [LabelText("XZ平面")]
        XZ,
        [LabelText("YZ平面")]
        YZ
    }

    [Flags]
    public enum FaceType
    {
        /// <summary>
        /// pos with offset (1, 0, 0)
        /// </summary>
        [LabelText("右(1, 0, 0)")]
        Right = 1 << 1,
        /// <summary>
        /// pos with offset (-1, 0, 0)
        /// </summary>
        [LabelText("左(-1, 0, 0)")]
        Left = 1 << 2,
        /// <summary>
        /// pos with offset (0, 1, 0)
        /// </summary>
        [LabelText("上(0, 1, 0)")]
        Up = 1 << 3,
        /// <summary>
        /// pos with offset (0, -1, 0)
        /// </summary>
        [LabelText("下(0, -1, 0)")]
        Down = 1 << 4,
        /// <summary>
        /// pos with offset (0, 0, 1)
        /// </summary>
        [LabelText("前(0, 0, 1)")]
        Forward = 1 << 5,
        /// <summary>
        /// pos with offset (0, 0, -1)
        /// </summary>
        [LabelText("后(0, 0, -1)")]
        Back = 1 << 6,
        /// <summary>
        /// AllDirectionFace
        /// </summary>
        [LabelText("所有朝向")]
        All = Right | Left | Up | Down | Forward | Back,
    }

    #endregion

    public static class FaceTypeFunc
    {
        public static FaceType Reverse(this FaceType face)
        {
            return face switch
            {
                FaceType.Right => FaceType.Left,
                FaceType.Left => FaceType.Right,
                FaceType.Up => FaceType.Down,
                FaceType.Down => FaceType.Up,
                FaceType.Forward => FaceType.Back,
                FaceType.Back => FaceType.Forward,
                _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
            };
        }
    }

    public static class MathFunc
    {
        public const double SQRT2 = 1.41421356237309504880168872420969807856967187537694807317667973799;
        public const double SQRT3 = 1.732050807568877293527446341505872366942805253810380628055806;

        public const float SQRT2f = 1.41421356237309504880168872420969807856967187537694807317667973799f;
        public const float SQRT3f = 1.732050807568877293527446341505872366942805253810380628055806f;

        #region Generic

        #region BasicMath

        #region Clamp

        #region Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int num, int min, int max)
        {
            return num.ClampMin(min).ClampMax(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampMax(this int num, int max)
        {
            if (num > max)
            {
                num = max;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampMin(this int num, int min)
        {
            if (num < min)
            {
                num = min;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int num, int length)
        {
            return num.Clamp(0, length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp01(this int num)
        {
            return Clamp(num, 0, 1);
        }


        #endregion

        #region Float

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float num, float min, float max)
        {
            if (num < min)
            {
                num = min;
            }
            else if (num > max)
            {
                num = max;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampMax(this float num, float max)
        {
            if (num > max)
            {
                num = max;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampMin(this float num, float min)
        {
            if (num < min)
            {
                num = min;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float num, float length)
        {
            return num.Clamp(0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(this float num)
        {
            return Clamp(num, 0, 1);
        }

        #endregion

        #region Double

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double num, double min, double max)
        {
            if (num < min)
            {
                num = min;
            }
            else if (num > max)
            {
                num = max;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampMax(this double num, double max)
        {
            if (num > max)
            {
                num = max;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampMin(this double num, double min)
        {
            if (num < min)
            {
                num = min;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double num, double length)
        {
            return num.Clamp(0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp01(this double num)
        {
            return Clamp(num, 0, 1);
        }

        #endregion

        #region Vector3

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(this Vector3 a, Vector3 min, Vector3 max)
        {
            return a.ClampMin(min).ClampMax(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(this Vector3 a, Vector3 size)
        {
            return a.ForeachNumber(size, (num, sizeNum) => num.Clamp(sizeNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ClampMin(this Vector3 a, Vector3 min)
        {
            return a.ForeachNumber(min, (num, minNum) => num.Clamp(minNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ClampMax(this Vector3 a, Vector3 max)
        {
            return a.ForeachNumber(max, (num, maxNum) => num.Clamp(maxNum));
        }

        #endregion

        #region Vector2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(this Vector2 a, Vector2 min, Vector2 max)
        {
            return a.ClampMin(min).ClampMax(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(this Vector2 a, Vector2 size)
        {
            return a.ForeachNumber(size, (num, sizeNum) => num.Clamp(sizeNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampMin(this Vector2 a, Vector2 min)
        {
            return a.ForeachNumber(min, (num, minNum) => num.Clamp(minNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampMax(this Vector2 a, Vector2 max)
        {
            return a.ForeachNumber(max, (num, maxNum) => num.Clamp(maxNum));
        }

        #endregion

        #region Vector3Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Clamp(this Vector3Int a, Vector3Int min, Vector3Int max)
        {
            return a.ClampMin(min).ClampMax(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Clamp(this Vector3Int a, Vector3Int size)
        {
            return a.ForeachNumber(size, (num, sizeNum) => num.Clamp(sizeNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ClampMin(this Vector3Int a, Vector3Int min)
        {
            return a.ForeachNumber(min, (num, minNum) => num.Clamp(minNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ClampMax(this Vector3Int a, Vector3Int max)
        {
            return a.ForeachNumber(max, (num, maxNum) => num.Clamp(maxNum));
        }

        #endregion

        #region Vector2Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Clamp(this Vector2Int a, Vector2Int min, Vector2Int max)
        {
            return a.ClampMin(min).ClampMax(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Clamp(this Vector2Int a, Vector2Int size)
        {
            return a.ForeachNumber(size, (num, sizeNum) => num.Clamp(sizeNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ClampMin(this Vector2Int a, Vector2Int min)
        {
            return a.ForeachNumber(min, (num, minNum) => num.Clamp(minNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ClampMax(this Vector2Int a, Vector2Int max)
        {
            return a.ForeachNumber(max, (num, maxNum) => num.Clamp(maxNum));
        }

        #endregion

        #region Vector4

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Clamp(this Vector4 a, Vector4 min, Vector4 max)
        {
            return a.ClampMin(min).ClampMax(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Clamp(this Vector4 a, Vector4 size)
        {
            return a.ForeachNumber(size, (num, sizeNum) => num.Clamp(sizeNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ClampMin(this Vector4 a, Vector4 min)
        {
            return a.ForeachNumber(min, (num, minNum) => num.Clamp(minNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ClampMax(this Vector4 a, Vector4 max)
        {
            return a.ForeachNumber(max, (num, maxNum) => num.Clamp(maxNum));
        }

        #endregion

        #region Color

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Clamp(this Color a, Color min, Color max)
        {
            return a.ClampMin(min).ClampMax(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Clamp(this Color a, Color size)
        {
            return a.ForeachNumber(size, (num, sizeNum) => num.Clamp(sizeNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ClampMin(this Color a, Color min)
        {
            return a.ForeachNumber(min, (num, minNum) => num.Clamp(minNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ClampMax(this Color a, Color max)
        {
            return a.ForeachNumber(max, (num, maxNum) => num.Clamp(maxNum));
        }

        #endregion

        #endregion

        #region Abs

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int num)
        {
            return Mathf.Abs(num);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(this float num)
        {
            return Mathf.Abs(num);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double num)
        {
            return Math.Abs(num);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Abs(this Vector4 a)
        {
            return a.ForeachNumber(num => num.Abs());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Abs(this Vector3Int a)
        {
            return a.ForeachNumber(num => num.Abs());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Abs(this Vector3 a)
        {
            return a.ForeachNumber(num => num.Abs());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Abs(this Vector2Int a)
        {
            return a.ForeachNumber(num => num.Abs());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs(this Vector2 a)
        {
            return a.ForeachNumber(num => num.Abs());
        }

        #endregion

        #region Modulus

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Modulus(this Vector2 vector)
        {
            return vector.magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Modulus(this Vector3 vector)
        {
            return vector.magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Modulus(this Vector2Int vector)
        {
            return vector.magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Modulus(this Vector3Int vector)
        {
            return vector.magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Modulus(this Vector4 vector)
        {
            return vector.magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Modulus(this Color color)
        {
            return ((Vector4)color.ConvertTo<Color>()).magnitude;
        }

        #endregion

        #region Distance

        #region For Single Value

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Distance(this int a, int b)
        {
            return (a - b).Abs();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this float a, float b)
        {
            return (a - b).Abs();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(this double a, double b)
        {
            return (a - b).Abs();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Color a, Color b, bool ignoreAlpha = false)
        {
            return ignoreAlpha ? Distance(a.To3D(), b.To3D()) : Distance((Vector4)a, (Vector4)b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector4 a, Vector4 b)
        {
            return a.EuclideanDistance(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector3Int a, Vector3Int b)
        {
            return a.EuclideanDistance(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector2Int a, Vector2Int b)
        {
            return a.EuclideanDistance(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector3 a, Vector3 b)
        {
            return a.EuclideanDistance(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector2 a, Vector2 b)
        {
            return a.EuclideanDistance(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EuclideanDistance(this Vector4 a, Vector4 b)
        {
            return Vector4.Distance(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EuclideanDistance(this Vector3Int a, Vector3Int b)
        {
            return Vector3Int.Distance(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EuclideanDistance(this Vector2Int a, Vector2Int b)
        {
            return Vector2Int.Distance(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EuclideanDistance(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EuclideanDistance(this Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistance(this Vector3Int a, Vector3Int b)
        {
            return (a - b).Abs().Sum();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
        {
            return (a - b).Abs().Sum();
        }

        #endregion

        #region For Enumerable

        #region Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinDistance<T>(this int origin, IEnumerable<T> enumerable, Func<T, int> selector)
        {
            var minDistance = int.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinDistance(this int origin, IEnumerable<int> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinDistance(this int origin, params int[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxDistance<T>(this int origin, IEnumerable<T> enumerable, Func<T, int> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxDistance(this int origin, IEnumerable<int> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxDistance(this int origin, params int[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #region Float

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance<T>(this float origin, IEnumerable<T> enumerable, Func<T, float> selector)
        {
            var minDistance = float.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this float origin, IEnumerable<float> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this float origin, params float[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance<T>(this float origin, IEnumerable<T> enumerable, Func<T, float> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this float origin, IEnumerable<float> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this float origin, params float[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #region Vector2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance<T>(this Vector2 origin, IEnumerable<T> enumerable, Func<T, Vector2> selector)
        {
            var minDistance = float.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector2 origin, IEnumerable<Vector2> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector2 origin, params Vector2[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance<T>(this Vector2 origin, IEnumerable<T> enumerable, Func<T, Vector2> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector2 origin, IEnumerable<Vector2> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector2 origin, params Vector2[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #region Vector2Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance<T>(this Vector2Int origin, IEnumerable<T> enumerable, Func<T, Vector2Int> selector)
        {
            var minDistance = float.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector2Int origin, IEnumerable<Vector2Int> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector2Int origin, params Vector2Int[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance<T>(this Vector2Int origin, IEnumerable<T> enumerable, Func<T, Vector2Int> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector2Int origin, IEnumerable<Vector2Int> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector2Int origin, params Vector2Int[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #region Vector3

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance<T>(this Vector3 origin, IEnumerable<T> enumerable, Func<T, Vector3> selector)
        {
            var minDistance = float.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector3 origin, IEnumerable<Vector3> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector3 origin, params Vector3[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance<T>(this Vector3 origin, IEnumerable<T> enumerable, Func<T, Vector3> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector3 origin, IEnumerable<Vector3> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector3 origin, params Vector3[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #region Vector3Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance<T>(this Vector3Int origin, IEnumerable<T> enumerable, Func<T, Vector3Int> selector)
        {
            var minDistance = float.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector3Int origin, IEnumerable<Vector3Int> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector3Int origin, params Vector3Int[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance<T>(this Vector3Int origin, IEnumerable<T> enumerable, Func<T, Vector3Int> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector3Int origin, IEnumerable<Vector3Int> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector3Int origin, params Vector3Int[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #region Vector4

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 MinDistanceOne<T>(this Vector4 origin, IEnumerable<T> enumerable, Func<T, Vector4> selector)
        {
            var list = enumerable.ToList();

            Note.note.AssertIsAbove(list.Count, 0, nameof(list.Count));

            var minDistance = float.MaxValue;
            Vector4 minDistanceOne = default;

            foreach (var t in list)
            {
                var one = selector(t);
                var distance = one.Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return one;
                    }

                    minDistance = distance;
                    minDistanceOne = one;
                }
            }

            return minDistanceOne;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 MinDistanceOne(this Vector4 origin, IEnumerable<Vector4> enumerable)
        {
            return origin.MinDistanceOne(enumerable, color => color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 MinDistanceOne(this Vector4 origin, params Vector4[] array)
        {
            return origin.MinDistanceOne(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 MaxDistanceOne<T>(this Vector4 origin, IEnumerable<T> enumerable, Func<T, Vector4> selector)
        {
            var list = enumerable.ToList();

            Note.note.AssertIsAbove(list.Count, 0, nameof(list.Count));

            float maxDistance = -1;
            Vector4 maxDistanceOne = default;

            foreach (var t in list)
            {
                var one = selector(t);
                var distance = one.Distance(origin);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistanceOne = one;
                }

            }

            return maxDistanceOne;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 MaxDistanceOne(this Vector4 origin, IEnumerable<Vector4> enumerable)
        {
            return origin.MaxDistanceOne(enumerable, color => color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 MaxDistanceOne(this Vector4 origin, params Vector4[] array)
        {
            return origin.MaxDistanceOne(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance<T>(this Vector4 origin, IEnumerable<T> enumerable, Func<T, Vector4> selector)
        {
            var minDistance = float.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector4 origin, IEnumerable<Vector4> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Vector4 origin, params Vector4[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance<T>(this Vector4 origin, IEnumerable<T> enumerable, Func<T, Vector4> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector4 origin, IEnumerable<Vector4> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Vector4 origin, params Vector4[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #region Color

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MinDistanceOne<T>(this Color origin, IEnumerable<T> enumerable, Func<T, Color> selector,
            bool ignoreAlpha = true)
        {
            var list = enumerable.ToList();

            Note.note.AssertIsAbove(list.Count, 0, nameof(list.Count));

            var minDistance = float.MaxValue;
            T minDistanceOne = default;

            foreach (var t in list)
            {
                var distance = selector(t).Distance(origin, ignoreAlpha);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return t;
                    }

                    minDistance = distance;
                    minDistanceOne = t;
                }
            }

            return minDistanceOne;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MinDistanceOne(this Color origin, IEnumerable<Color> enumerable, bool ignoreAlpha = true)
        {
            return origin.MinDistanceOne(enumerable, color => color, ignoreAlpha);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MinDistanceOne(this Color origin, params Color[] array)
        {
            return origin.MinDistanceOne(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MaxDistanceOne<T>(this Color origin, IEnumerable<T> enumerable, Func<T, Color> selector, 
            bool ignoreAlpha = true)
        {
            var list = enumerable.ToList();

            Note.note.AssertIsAbove(list.Count, 0, nameof(list.Count));

            float maxDistance = -1;
            T maxDistanceOne = default;

            foreach (var t in list)
            {
                var distance = selector(t).Distance(origin, ignoreAlpha);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistanceOne = t;
                }
                
            }

            return maxDistanceOne;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MaxDistanceOne(this Color origin, IEnumerable<Color> enumerable, bool ignoreAlpha = true)
        {
            return origin.MaxDistanceOne(enumerable, color => color, ignoreAlpha);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MaxDistanceOne(this Color origin, params Color[] array)
        {
            return origin.MaxDistanceOne(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance<T>(this Color origin, IEnumerable<T> enumerable, Func<T, Color> selector)
        {
            var minDistance = float.MaxValue;

            foreach (var t in enumerable)
            {
                var distance = selector(t).Distance(origin);

                if (distance < minDistance)
                {
                    if (minDistance == 0)
                    {
                        return 0;
                    }

                    minDistance = distance;
                }
            }

            return minDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Color origin, IEnumerable<Color> enumerable)
        {
            return origin.MinDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinDistance(this Color origin, params Color[] array)
        {
            return origin.MinDistance(array.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance<T>(this Color origin, IEnumerable<T> enumerable, Func<T, Color> selector)
        {
            return enumerable.Select(t => selector(t).Distance(origin)).Prepend(0).Max();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Color origin, IEnumerable<Color> enumerable)
        {
            return origin.MaxDistance(enumerable, i => i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxDistance(this Color origin, params Color[] array)
        {
            return origin.MaxDistance(array.AsEnumerable());
        }

        #endregion

        #endregion

        #endregion

        #region Multiply

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Multiply(this Vector2Int vector, float a)
        {
            return vector.ForeachNumber(num => (num * a).Round());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Multiply(this Vector2Int a, Vector2Int b)
        {
            return a.ForeachNumber(b, (numA, numB) => numA * numB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Multiply(this Vector3Int vector, float a)
        {
            return vector.ForeachNumber(num => (num * a).Round());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Multiply(this Vector3Int a, Vector3Int b)
        {
            return a.ForeachNumber(b, (numA, numB) => numA * numB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Multiply(this Vector2 a, Vector2 b)
        {
            return a.ForeachNumber(b, (numA, numB) => numA * numB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Multiply(this Vector3 a, Vector3 b)
        {
            return a.ForeachNumber(b, (numA, numB) => numA * numB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Multiply(this Vector4 a, Vector4 b)
        {
            return a.ForeachNumber(b, (numA, numB) => numA * numB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Multiply(this Color a, Color b)
        {
            return a.ForeachNumber(b, (numA, numB) => numA * numB);
        }

        #endregion

        #region Divide

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Divide(this int dividend, int divisor)
        {
            if (dividend < 0)
            {
                return (dividend + 1) / divisor - 1;
            }
            else
            {
                return dividend / divisor;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Divide(this float dividend, float divisor)
        {
            if (divisor == 0 && dividend == 0)
            {
                return 1;
            }

            return dividend / divisor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Divide(this double dividend, double divisor)
        {
            if (divisor == 0 && dividend == 0)
            {
                return 1;
            }

            return dividend / divisor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Divide(this Vector3Int dividend, Vector3Int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Divide(this Vector3Int dividend, int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Divide(this Vector2Int dividend, Vector2Int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Divide(this Vector2Int dividend, int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Divide(this Vector3 dividend, Vector3 divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Divide(this Vector3 dividend, float divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Divide(this Vector2 dividend, Vector2 divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Divide(this Vector2 dividend, float divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Divide(this Vector4 dividend, Vector4 divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Divide(this Vector4 dividend, float divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Divide(this Color dividend, Color divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Divide(this Color dividend, float divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Divide(divisorElement));
        }

        #endregion

        #region Modulo

        /// <summary>
        /// 7 Modulo 3 = 1
        /// -5 Modulo 3 = 2 rather than -1
        /// -6 Modulo 3 = 0
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Modulo(this int dividend, int divisor)
        {
            int temp = dividend % divisor;
            if (dividend < 0)
            {
                if (temp == 0)
                {
                    return 0;
                }
                else
                {
                    return divisor - Mathf.Abs(temp);
                }
            }
            else
            {
                return temp;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Modulo(this Vector3Int dividend, int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Modulo(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Modulo(this Vector2Int dividend, int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Modulo(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Modulo(this Vector3Int dividend, Vector3Int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Modulo(divisorElement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Modulo(this Vector2Int dividend, Vector2Int divisor)
        {
            return ForeachNumber(dividend, divisor, (dividendElement, divisorElement) =>
                dividendElement.Modulo(divisorElement));
        }

        #region For Enumerable

        public static IEnumerable<int> ModuloRange(int start, int end, int divisor, bool includingBound = false)
        {
            int startIndex = start.Divide(divisor);
            int endIndex = end.Divide(divisor);

            if (includingBound == false)
            {
                startIndex++;
                endIndex--;
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                yield return i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> ModuloRange(Vector3Int start, Vector3Int end, Vector3Int divisor, bool includingBound = false)
        {
            foreach (var x in ModuloRange(start.x, end.x, divisor.x, includingBound))
            {
                foreach (var y in ModuloRange(start.y, end.y, divisor.y, includingBound))
                {
                    foreach (var z in ModuloRange(start.z, end.z, divisor.z, includingBound))
                    {
                        yield return new(x, y, z);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Pow

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pow(this int toPow, float power)
        {
            return Mathf.Pow(toPow, power).Round();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this float toPow, float power)
        {
            return Mathf.Pow(toPow, power);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pow(this double toPow, float power)
        {
            return Math.Pow(toPow, power);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Pow(this Vector2 toPow, float power)
        {
            return ForeachNumber(toPow, f => Mathf.Pow(f, power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Pow(this Vector2Int toPow, float power)
        {
            return ForeachNumber(toPow, f => Mathf.Pow(f, power).Round());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Pow(this Vector3 toPow, float power)
        {
            return ForeachNumber(toPow, f => Mathf.Pow(f, power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Pow(this Vector3Int toPow, float power)
        {
            return ForeachNumber(toPow, f => Mathf.Pow(f, power).Round());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Pow(this Vector4 toPow, float power)
        {
            return ForeachNumber(toPow, f => Mathf.Pow(f, power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Pow(this Color toPow, float power)
        {
            return ForeachNumber(toPow, f => Mathf.Pow(f, power));
        }

        #endregion

        #region Lerp

        #region NormalLerp

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Lerp(this int from, int to, float t)
        {
            return Mathf.Lerp(from, to, t).Round();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(this float from, float to, float t)
        {
            return Mathf.Lerp(from, to, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(this double from, double to, float t)
        {
            return from + (to - from) * t.Clamp01();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Lerp(this Vector4 a, Vector4 b, float t)
        {
            return Vector4.Lerp(a, b, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Lerp(this Color a, Color b, float t)
        {
            return Color.Lerp(a, b, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(this Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(this Vector2 a, Vector2 b, float t)
        {
            return Vector2.Lerp(a, b, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Lerp(this Vector3Int a, Vector3Int b, float t)
        {
            return a.ForeachNumber(b, (aNum, bNum) => aNum.Lerp(bNum, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Lerp(this Vector2Int a, Vector2Int b, float t)
        {
            return a.ForeachNumber(b, (aNum, bNum) => aNum.Lerp(bNum, t));
        }

        #endregion

        #region WithPower

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Lerp(this int from, int to, float t, float power)
        {
            return Mathf.Lerp(from, to, t.Pow(power)).Round();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(this float from, float to, float t, float power)
        {
            return Mathf.Lerp(from, to, t.Pow(power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(this double from, double to, float t, float power)
        {
            return from + (to - from) * t.Clamp01().Pow(power);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Lerp(this Vector4 a, Vector4 b, float t, float power)
        {
            return Vector4.Lerp(a, b, t.Pow(power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Lerp(this Color a, Color b, float t, float power)
        {
            return Color.Lerp(a, b, t.Pow(power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(this Vector3 a, Vector3 b, float t, float power)
        {
            return Vector3.Lerp(a, b, t.Pow(power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(this Vector2 a, Vector2 b, float t, float power)
        {
            return Vector2.Lerp(a, b, t.Pow(power));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Lerp(this Vector3Int a, Vector3Int b, float t, float power)
        {
            return a.ForeachNumber(b, (aNum, bNum) => aNum.Lerp(bNum, t.Pow(power)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Lerp(this Vector2Int a, Vector2Int b, float t, float power)
        {
            return a.ForeachNumber(b, (aNum, bNum) => aNum.Lerp(bNum, t.Pow(power)));
        }

        #endregion

        #endregion

        #region Round

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this float value)
        {
            return Mathf.RoundToInt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this double value)
        {
            return (int)Math.Round(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Round(this Vector2 vector)
        {
            return vector.ForeachNumber(num => num.Round());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Round(this Vector3 vector)
        {
            return vector.ForeachNumber(num => num.Round());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Round(this Vector4 vector)
        {
            return vector.ForeachNumber(num => num.Round());
        }

        #endregion

        #region Ceiling

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(this float value)
        {
            return Mathf.CeilToInt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(this double value)
        {
            return (int)Math.Ceiling(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Ceiling(this Vector2 vector)
        {
            return vector.ForeachNumber(num => num.Ceiling());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Ceiling(this Vector3 vector)
        {
            return vector.ForeachNumber(num => num.Ceiling());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Ceiling(this Vector4 vector)
        {
            return vector.ForeachNumber(num => num.Ceiling());
        }

        #endregion

        #region Floor

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(this double value)
        {
            return (int)Math.Floor(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Floor(this Vector2 vector)
        {
            return vector.ForeachNumber(num => num.Floor());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Floor(this Vector3 vector)
        {
            return vector.ForeachNumber(num => num.Floor());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Floor(this Vector4 vector)
        {
            return vector.ForeachNumber(num => num.Floor());
        }

        #endregion

        #region Products

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Products(this Vector3Int a)
        {
            return a.x * a.y * a.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Products(this Vector3 a)
        {
            return a.x * a.y * a.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Products(this Vector2Int a)
        {
            return a.x * a.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Products(this Vector2 a)
        {
            return a.x * a.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Products(this Vector4 a)
        {
            return a.x * a.y * a.z * a.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Products(this Color a)
        {
            return a.r * a.g * a.b * a.a;
        }

        #endregion

        #region Sum

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum(this Vector3Int a)
        {
            return a.x + a.y + a.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector3 a)
        {
            return a.x + a.y + a.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum(this Vector2Int a)
        {
            return a.x + a.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector2 a)
        {
            return a.x + a.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector4 a)
        {
            return a.x + a.y + a.z + a.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Color a)
        {
            return a.r + a.g + a.b + a.a;
        }

        #endregion

        #region Normalize

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Normalize(this double t, double min, double max)
        {
            return (t - min) / (max - min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalize(this float t, float min, float max)
        {
            return (t - min) / (max - min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalize(this int t, int min, int max)
        {
            return (t - min).F() / (max - min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(this Vector2 t, Vector2 min, Vector2 max)
        {
            return t.ForeachNumber(min, max, (num, minNum, maxNum) => num.Normalize(minNum, maxNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalize(this Vector3 t, Vector3 min, Vector3 max)
        {
            return t.ForeachNumber(min, max, (num, minNum, maxNum) => num.Normalize(minNum, maxNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Normalize(this Vector4 t, Vector4 min, Vector4 max)
        {
            return t.ForeachNumber(min, max, (num, minNum, maxNum) => num.Normalize(minNum, maxNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(this Vector2Int t, Vector2Int min, Vector2Int max)
        {
            return t.ForeachNumber(min, max, (num, minNum, maxNum) => num.Normalize(minNum, maxNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalize(this Vector3Int t, Vector3Int min, Vector3Int max)
        {
            return t.ForeachNumber(min, max, (num, minNum, maxNum) => num.Normalize(minNum, maxNum));
        }

        #endregion

        #region All

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Color a, Func<float, bool> func)
        {
            return func(a.r) && func(a.g) && func(a.b) && func(a.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector4 a, Func<float, bool> func)
        {
            return func(a.x) && func(a.y) && func(a.z) && func(a.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector3Int a, Func<int, bool> func)
        {
            return func(a.x) && func(a.y) && func(a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector3 a, Func<float, bool> func)
        {
            return func(a.x) && func(a.y) && func(a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector2Int a, Func<int, bool> func)
        {
            return func(a.x) && func(a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector2 a, Func<float, bool> func)
        {
            return func(a.x) && func(a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Color a, Color b, Func<float, float, bool> func)
        {
            return func(a.r, b.r) && func(a.g, b.g) && func(a.b, b.b) && func(a.a, b.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector4 a, Vector4 b, Func<float, float, bool> func)
        {
            return func(a.x, b.x) && func(a.y, b.y) && func(a.z, b.z) && func(a.w, b.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector3Int a, Vector3Int b, Func<int, int, bool> func)
        {
            return func(a.x, b.x) && func(a.y, b.y) && func(a.z, b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector3 a, Vector3 b, Func<float, float, bool> func)
        {
            return func(a.x, b.x) && func(a.y, b.y) && func(a.z, b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector2Int a, Vector2Int b, Func<int, int, bool> func)
        {
            return func(a.x, b.x) && func(a.y, b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this Vector2 a, Vector2 b, Func<float, float, bool> func)
        {
            return func(a.x, b.x) && func(a.y, b.y);
        }

        #endregion

        #region Any

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Color a, Func<float, bool> func)
        {
            return func(a.r) || func(a.g) || func(a.b) || func(a.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector4 a, Func<float, bool> func)
        {
            return func(a.x) || func(a.y) || func(a.z) || func(a.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector3Int a, Func<int, bool> func)
        {
            return func(a.x) || func(a.y) || func(a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector3 a, Func<float, bool> func)
        {
            return func(a.x) || func(a.y) || func(a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector2Int a, Func<int, bool> func)
        {
            return func(a.x) || func(a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector2 a, Func<float, bool> func)
        {
            return func(a.x) || func(a.y);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Color a, Color b, Func<float, float, bool> func)
        {
            return func(a.r, b.r) || func(a.g, b.g) || func(a.b, b.b) || func(a.a, b.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector4 a, Vector4 b, Func<float, float, bool> func)
        {
            return func(a.x, b.x) || func(a.y, b.y) || func(a.z, b.z) || func(a.w, b.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector3Int a, Vector3Int b, Func<int, int, bool> func)
        {
            return func(a.x, b.x) || func(a.y, b.y) || func(a.z, b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector3 a, Vector3 b, Func<float, float, bool> func)
        {
            return func(a.x, b.x) || func(a.y, b.y) || func(a.z, b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector2Int a, Vector2Int b, Func<int, int, bool> func)
        {
            return func(a.x, b.x) || func(a.y, b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this Vector2 a, Vector2 b, Func<float, float, bool> func)
        {
            return func(a.x, b.x) || func(a.y, b.y);
        }

        #endregion

        #region Symmetric

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PointSymmetric(this double a, double point = 0)
        {
            return point + point - a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PointSymmetric(this float a, float point = 0)
        {
            return point + point - a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PointSymmetric(this int a, int point = 0)
        {
            return point + point - a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int XAxisymmetric(this Vector2Int a, int xAxis = 0)
        {
            return new(xAxis + xAxis - a.x, a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int YAxisymmetric(this Vector2Int a, int yAxis = 0)
        {
            return new(a.x, yAxis + yAxis - a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int PointSymmetric(this Vector2Int a, Vector2Int point = default)
        {
            return a.ForeachNumber(point, (num, pointNum) => num.PointSymmetric(pointNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector2Int> Symmetric(this Vector2Int a, Vector2Int point = default, bool includingSelf = true)
        {
            if (includingSelf == true)
            {
                yield return a;
            }
            yield return a.XAxisymmetric(point.x);
            yield return a.YAxisymmetric(point.y);
            yield return a.PointSymmetric(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int XAxisymmetric(this Vector3Int a, int xAxis = 0)
        {
            return new(xAxis + xAxis - a.x, a.y, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int YAxisymmetric(this Vector3Int a, int yAxis = 0)
        {
            return new(a.x, yAxis + yAxis - a.y, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ZAxisymmetric(this Vector3Int a, int zAxis = 0)
        {
            return new(a.x, a.y, zAxis + zAxis - a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int XYAxisymmetric(this Vector3Int a, int xAxis = 0, int yAxis = 0)
        {
            return new(xAxis + xAxis - a.x, yAxis + yAxis - a.y, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int XZAxisymmetric(this Vector3Int a, int xAxis = 0, int zAxis = 0)
        {
            return new(xAxis + xAxis - a.x, a.y, zAxis + zAxis - a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int YZAxisymmetric(this Vector3Int a, int yAxis = 0, int zAxis = 0)
        {
            return new(a.x, yAxis + yAxis - a.y, zAxis + zAxis - a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int PointSymmetric(this Vector3Int a, Vector3Int point = default)
        {
            return a.ForeachNumber(point, (num, pointNum) => num.PointSymmetric(pointNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> Symmetric(this Vector3Int a, Vector3Int point = default, bool includingSelf = true)
        {
            if (includingSelf == true)
            {
                yield return a;
            }
            yield return a.XAxisymmetric(point.x);
            yield return a.YAxisymmetric(point.y);
            yield return a.ZAxisymmetric(point.z);

            yield return a.XYAxisymmetric(point.x, point.y);
            yield return a.XZAxisymmetric(point.x, point.z);
            yield return a.YZAxisymmetric(point.y, point.z);

            yield return a.PointSymmetric(point);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XAxisymmetric(this Vector2 a, float xAxis = 0)
        {
            return new(xAxis + xAxis - a.x, a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YAxisymmetric(this Vector2 a, float yAxis = 0)
        {
            return new(a.x, yAxis + yAxis - a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PointSymmetric(this Vector2 a, Vector2 point = default)
        {
            return a.ForeachNumber(point, (num, pointNum) => num.PointSymmetric(pointNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector2> Symmetric(this Vector2 a, Vector2 point = default, bool includingSelf = true)
        {
            if (includingSelf == true)
            {
                yield return a;
            }
            yield return a.XAxisymmetric(point.x);
            yield return a.YAxisymmetric(point.y);
            yield return a.PointSymmetric(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 XAxisymmetric(this Vector3 a, float xAxis = 0)
        {
            return new(xAxis + xAxis - a.x, a.y, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 YAxisymmetric(this Vector3 a, float yAxis = 0)
        {
            return new(a.x, yAxis + yAxis - a.y, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ZAxisymmetric(this Vector3 a, float zAxis = 0)
        {
            return new(a.x, a.y, zAxis + zAxis - a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 XYAxisymmetric(this Vector3 a, float xAxis = 0, float yAxis = 0)
        {
            return new(xAxis + xAxis - a.x, yAxis + yAxis - a.y, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 XZAxisymmetric(this Vector3 a, float xAxis = 0, float zAxis = 0)
        {
            return new(xAxis + xAxis - a.x, a.y, zAxis + zAxis - a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 YZAxisymmetric(this Vector3 a, float yAxis = 0, float zAxis = 0)
        {
            return new(a.x, yAxis + yAxis - a.y, zAxis + zAxis - a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 PointSymmetric(this Vector3 a, Vector3 point = default)
        {
            return a.ForeachNumber(point, (num, pointNum) => num.PointSymmetric(pointNum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3> Symmetric(this Vector3 a, Vector3 point = default, bool includingSelf = true)
        {
            if (includingSelf == true)
            {
                yield return a;
            }
            yield return a.XAxisymmetric(point.x);
            yield return a.YAxisymmetric(point.y);
            yield return a.ZAxisymmetric(point.z);

            yield return a.XYAxisymmetric(point.x, point.y);
            yield return a.XZAxisymmetric(point.x, point.z);
            yield return a.YZAxisymmetric(point.y, point.z);

            yield return a.PointSymmetric(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 PointSymmetric(this Vector4 a, Vector4 point = default)
        {
            return a.ForeachNumber(point, (num, pointNum) => num.PointSymmetric(pointNum));
        }

        #endregion

        #region Min

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int a, int b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Min(this long a, long b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Min(this short a, short b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Min(double a, double b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            Vector2 minVector = new()
            {
                x = a.x < b.x ? a.x : b.x,
                y = a.y < b.y ? a.y : b.y
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            Vector3 minVector = new()
            {
                x = a.x < b.x ? a.x : b.x,
                y = a.y < b.y ? a.y : b.y,
                z = a.z < b.z ? a.z : b.z
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Min(Vector2Int a, Vector2Int b)
        {
            Vector2Int minVector = new()
            {
                x = a.x < b.x ? a.x : b.x,
                y = a.y < b.y ? a.y : b.y
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Min(Vector3Int a, Vector3Int b)
        {
            Vector3Int minVector = new()
            {
                x = a.x < b.x ? a.x : b.x,
                y = a.y < b.y ? a.y : b.y,
                z = a.z < b.z ? a.z : b.z
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Min(Vector4 a, Vector4 b)
        {
            Vector4 minVector = new()
            {
                x = a.x < b.x ? a.x : b.x,
                y = a.y < b.y ? a.y : b.y,
                z = a.z < b.z ? a.z : b.z,
                w = a.w < b.w ? a.w : b.w
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Min(Color a, Color b)
        {
            Color minColor = new()
            {
                r = a.r < b.r ? a.r : b.r,
                g = a.g < b.g ? a.g : b.g,
                b = a.b < b.b ? a.b : b.b,
                a = a.a < b.a ? a.a : b.a
            };
            return minColor;
        }

        #endregion

        #region Max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int a, int b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Max(this long a, long b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Max(this short a, short b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            Vector2 minVector = new()
            {
                x = a.x > b.x ? a.x : b.x,
                y = a.y > b.y ? a.y : b.y
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            Vector3 minVector = new()
            {
                x = a.x > b.x ? a.x : b.x,
                y = a.y > b.y ? a.y : b.y,
                z = a.z > b.z ? a.z : b.z
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Max(Vector2Int a, Vector2Int b)
        {
            Vector2Int minVector = new()
            {
                x = a.x > b.x ? a.x : b.x,
                y = a.y > b.y ? a.y : b.y
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Max(Vector3Int a, Vector3Int b)
        {
            Vector3Int minVector = new()
            {
                x = a.x > b.x ? a.x : b.x,
                y = a.y > b.y ? a.y : b.y,
                z = a.z > b.z ? a.z : b.z
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Max(Vector4 a, Vector4 b)
        {
            Vector4 minVector = new()
            {
                x = a.x > b.x ? a.x : b.x,
                y = a.y > b.y ? a.y : b.y,
                z = a.z > b.z ? a.z : b.z,
                w = a.w > b.w ? a.w : b.w
            };
            return minVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Max(Color a, Color b)
        {
            Color minColor = new()
            {
                r = a.r > b.r ? a.r : b.r,
                g = a.g > b.g ? a.g : b.g,
                b = a.b > b.b ? a.b : b.b,
                a = a.a > b.a ? a.a : b.a
            };
            return minColor;
        }

        #endregion

        #endregion


        #endregion

        #region Number

        #region Basic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float F(this int num)
        {
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float F(this double num)
        {
            return (float)num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            (lhs, rhs) = (rhs, lhs);
        }

        #endregion

        #region Repeat

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Repeat(this int num, int min, int max)
        {
            return num.Modulo(max - min + 1) + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Repeat(this int num, int length)
        {
            return num.Repeat(0, length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PingPong(this int num, int length)
        {
            length--;
            return length - (num.Repeat(length + length) - length).Abs();
        }

        #endregion

        #region Compare

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T Max<T>(this T a, T b) where T : IComparable
        //{
        //    return a.Above(b) ? a : b;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T Min<T>(this T a, T b) where T : IComparable
        //{
        //    return a.Below(b) ? a : b;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T Max<T>(this T a, params T[] others) where T : IComparable
        //{
        //    return others.Append(a).Max();
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T Min<T>(this T a, params T[] others) where T : IComparable
        //{
        //    return others.Append(a).Min();
        //}

        /// <summary>
        /// return true if num is in range [min, max]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BetweenInclusive<T>(this T num, T min, T max) where T : IComparable
        {
            return num.CompareTo(min) >= 0 && num.CompareTo(max) <= 0;
        }

        /// <summary>
        /// return true if num is in range (min, max)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BetweenExclusive<T>(this T num, T min, T max) where T : IComparable
        {
            return num.CompareTo(min) > 0 && num.CompareTo(max) < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Below<T>(this T num, T comparison) where T : IComparable
        {
            return num.CompareTo(comparison) < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BelowOrEqual<T>(this T num, T comparison) where T : IComparable
        {
            return num.CompareTo(comparison) <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Above<T>(this T num, T comparison) where T : IComparable
        {
            return num.CompareTo(comparison) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AboveOrEqual<T>(this T num, T comparison) where T : IComparable
        {
            return num.CompareTo(comparison) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<int> GetIndicesOfMaxValues<TItem, TValue>(this IList<TItem> list, Func<TItem, TValue> func)
            where TValue : IComparable
        {
            List<int> indices = new();
            TValue maxValue = default;

            bool hasInitialized = false;
            for (int i = 0; i < list.Count; i++)
            {
                var value = func(list[i]);

                if (hasInitialized == false)
                {
                    maxValue = value;

                    hasInitialized = true;
                }

                if (value.Above(maxValue))
                {
                    maxValue = value;
                    indices.Clear();
                }

                if (value.Equals(maxValue))
                {
                    indices.Add(i);
                }
            }

            return indices;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<int> GetIndicesOfMinValues<TItem, TValue>(this IList<TItem> list, Func<TItem, TValue> func)
            where TValue : IComparable
        {
            List<int> indices = new();
            TValue minValue = default;

            bool hasInitialized = false;
            for (int i = 0; i < list.Count; i++)
            {
                var value = func(list[i]);

                if (hasInitialized == false)
                {
                    minValue = value;

                    hasInitialized = true;
                }

                if (value.Below(minValue))
                {
                    minValue = value;
                    indices.Clear();
                }

                if (value.Equals(minValue))
                {
                    indices.Add(i);
                }
            }

            return indices;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<int> GetIndicesOfMaxValues<TValue>(this IList<TValue> list)
            where TValue : IComparable
        {
            return list.GetIndicesOfMaxValues(item => item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<int> GetIndicesOfMinValues<TValue>(this IList<TValue> list)
            where TValue : IComparable
        {
            return list.GetIndicesOfMinValues(item => item);
        }
        #endregion

        #region Caculate

        #region Sum

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum<T>(this IEnumerable<T> enumerable, Func<T, int> selector, int fromIndex, int endIndex)
        {
            int result = 0;
            foreach (var (index, item) in enumerable.Enumerate())
            {
                if (index.BetweenInclusive(fromIndex, endIndex))
                {
                    result += selector(item);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum<T>(this IEnumerable<T> enumerable, Func<T, float> selector, int fromIndex, int endIndex)
        {
            float result = 0;
            foreach (var (index, item) in enumerable.Enumerate())
            {
                if (index.BetweenInclusive(fromIndex, endIndex))
                {
                    result += selector(item);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum<T>(this IEnumerable<T> enumerable, Func<T, int> selector, int length)
        {
            return enumerable.Sum(selector, 0, length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum<T>(this IEnumerable<T> enumerable, Func<T, float> selector, int length)
        {
            return enumerable.Sum(selector, 0, length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum(this IEnumerable<int> enumerable, int fromIndex, int endIndex)
        {
            return enumerable.Sum(item => item, fromIndex, endIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this IEnumerable<float> enumerable, int fromIndex, int endIndex)
        {
            return enumerable.Sum(item => item, fromIndex, endIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum(this IEnumerable<int> enumerable, int length)
        {
            return enumerable.Sum(item => item, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this IEnumerable<float> enumerable, int length)
        {
            return enumerable.Sum(item => item, length);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(params bool[] args)
        {
            return args.Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(params bool[] args)
        {
            return args.Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int QuickPow(int a, int k, int p)
        {
            long res = 1;
            while (k != 0)
            {

                if ((k & 1) != 0) res = res * a % p;
                a = a * a % p;
                k >>= 1;
            }
            return (int)res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Percent(this float t, float min, float max)
        {
            float percent = t.Normalize(min, max).Clamp(0, 1);

            return percent * 100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Percent(this float t)
        {
            return t.Percent(0, 100);
        }

        #endregion

        #region Combinatorial

        public static int[,] combinatorialNumberPresets;

        public static bool isPresetCombinatorialNumbers = false;

        public static int combinatorialNumberPresetSize = 100;

        /// <summary>
        /// choose n items from total m items
        /// </summary>
        /// <param name="n">the amount of items you want to choose</param>
        /// <param name="m">total amount of items</param>
        /// <returns></returns>
        public static int CombinatorialNumber(int n, int m)
        {
            if (isPresetCombinatorialNumbers == false)
            {
                combinatorialNumberPresets = new int[combinatorialNumberPresetSize, combinatorialNumberPresetSize];

                for (int j = 0; j < combinatorialNumberPresetSize; j++)
                {
                    for (int i = 0; i <= j; i++)
                    {
                        if (i == 0 || i == j)
                        {
                            combinatorialNumberPresets[i, j] = 1;
                        }
                        else
                        {
                            combinatorialNumberPresets[i, j] = combinatorialNumberPresets[i, j - 1] + combinatorialNumberPresets[i - 1, j - 1];
                        }

                    }
                }

                isPresetCombinatorialNumbers = true;

                if (true)
                {
                    for (int j = 0; j < combinatorialNumberPresetSize; j++)
                    {
                        string log = "";
                        for (int i = 0; i <= j; i++)
                        {
                            log += combinatorialNumberPresets[i, j].ToString() + " ";

                        }
                        Debug.Log(log);
                    }
                }

            }

            if (n < 0 || m <= 0 || n > m)
            {
                return 0;
            }
            if (n == 0 || n == m)
            {
                return 1;
            }
            else if (m <= combinatorialNumberPresetSize)
            {
                return combinatorialNumberPresets[n, m];
            }
            else
            {
                return CombinatorialNumber(n, m - 1) + CombinatorialNumber(n - 1, m - 1);
            }

        }

        public static int GetCombinationNumbers<T>(this List<T> target)
        {
            return Mathf.RoundToInt(Mathf.Pow(2, target.Count));
        }

        public static IEnumerable<List<T>> GetCombinations<T>(this List<T> target, int amount, int lastNum)
        {

            if (amount <= 0)
            {
                yield break;
            }
            else if (lastNum < 0)
            {
                yield break;
            }
            else if (amount > lastNum + 1)
            {
                yield break;
            }
            else if (lastNum == 0)
            {
                yield return new() { target[0] };
            }
            else if (amount > 1)
            {
                for (int i = amount - 1; i <= lastNum; i++)
                {
                    foreach (var subResult in target.GetCombinations(amount - 1, i - 1))
                    {
                        List<T> newResult = new() { target[i] };

                        newResult.AddRange(subResult);

                        yield return newResult;
                    }
                }
            }
            else
            {
                for (int i = 0; i <= lastNum; i++)
                {
                    yield return new() { target[i] };
                }
            }

        }

        public static IEnumerable<List<T>> GetCombinations<T>(this List<T> target, int amount)
        {
            if (amount <= 0)
            {
                return new List<List<T>>();
            }
            else if (amount >= target.Count)
            {
                return new List<List<T>>() { target };
            }
            return target.GetCombinations(amount, target.Count - 1);
        }

        public static IEnumerable<List<T>> GetCombinations<T>(this List<T> target, bool includingNull = true)
        {
            if (includingNull == true)
            {
                yield return new();
            }

            for (int i = 1; i <= target.Count; i++)
            {
                foreach (var result in target.GetCombinations(i))
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<List<T>> GenerateCombinations<T>(this List<T> allElements, int resultLength)
        {
            if (resultLength <= 0)
            {
                yield break;
            }

            if (resultLength == 1)
            {
                foreach (var element in allElements)
                {
                    yield return new() { element };
                }
            }
            else
            {
                foreach (var smallCombination in GenerateCombinations<T>(allElements, resultLength - 1))
                {
                    foreach (var element in allElements)
                    {
                        var newCombination = new List<T>();

                        newCombination.AddRange(smallCombination);
                        newCombination.Add(element);

                        yield return newCombination;
                    }
                    
                }
            }
        }

        #endregion

        #region Number Theory

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreCoprime(params int[] ints)
        {
            return ints.GCD() == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreCoprime(this IEnumerable<int> enumerable)
        {
            return enumerable.GCD() == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GCD(params int[] ints)
        {
            return ints.GCD();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GCD(this IEnumerable<int> enumerable)
        {
            try
            {
                return enumerable.Aggregate(GCD);
            }
            catch (InvalidOperationException)
            {
                return 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GCD(this int a, int b)
        {

            // GCD(0, b) == b; GCD(a, 0) == a,
            // GCD(0, 0) == 0
            if (a == 0)
                return b;
            if (b == 0)
                return a;

            // Finding K, where K is the greatest
            // power of 2 that divides both a and b
            int k;
            for (k = 0; ((a | b) & 1) == 0; ++k)
            {
                a >>= 1;
                b >>= 1;
            }

            // Dividing a by 2 until a becomes odd
            while ((a & 1) == 0)
                a >>= 1;

            // From here on, 'a' is always odd
            do
            {
                // If b is even, remove
                // all factor of 2 in b
                while ((b & 1) == 0)
                    b >>= 1;

                /* Now a and b are both odd. Swap
                if necessary so a <= b, then set
                b = b - a (which is even).*/
                if (a > b)
                {

                    // Swap u and v.
                    (a, b) = (b, a);
                }

                b = (b - a);
            } while (b != 0);

            /* restore common factors of 2 */
            return a << k;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LCM(params int[] ints)
        {
            return ints.LCM();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LCM(this IEnumerable<int> enumerable)
        {
            try
            {
                return enumerable.Aggregate(LCM);
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LCM(int a, int b)
        {
            int gcd = GCD(a, b);
            return a / gcd * b;
        }

        #endregion

        #region Bezier

        public static Vector3 Bezier(float t, Vector3 p0, Vector3 p1)
        {
            return (1 - t) * p0 + t * p1;
        }

        public static Vector3 Bezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 p0p1 = (1 - t) * p0 + t * p1;
            Vector3 p1p2 = (1 - t) * p1 + t * p2;
            Vector3 temp = (1 - t) * p0p1 + t * p1p2;
            return temp;
        }

        public static Vector3 Bezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 p0p1 = (1 - t) * p0 + t * p1;
            Vector3 p1p2 = (1 - t) * p1 + t * p2;
            Vector3 p2p3 = (1 - t) * p2 + t * p3;
            Vector3 p0p1p2 = (1 - t) * p0p1 + t * p1p2;
            Vector3 p1p2p3 = (1 - t) * p1p2 + t * p2p3;
            var temp = (1 - t) * p0p1p2 + t * p1p2p3;
            return temp;
        }

        public static Vector3 Bezier(float t, List<Vector3> p)
        {
            if (p.Count < 2)
                return p[0];
            var newP = new List<Vector3>();
            for (int i = 0; i < p.Count - 1; i++)
            {
                // Debug.DrawLine(p[i], p[i + 1]);

                Vector3 p0p1 = (1 - t) * p[i] + t * p[i + 1];
                newP.Add(p0p1);
            }
            return Bezier(t, newP);
        }

        public static float BezierLength(float t, Vector2 vecA, Vector2 vecB, Vector2 vecC)
        {
            Vector2 va = vecA + vecC - 2 * vecB;
            Vector2 vb = 2 * (vecB - vecA);
            float A = 4 * (va.x * va.x + va.y * va.y);
            float B = 4 * (va.x * vb.x + va.y * vb.y);
            float C = vb.x * vb.x + vb.y * vb.y;

            float sa = Mathf.Sqrt(A);
            float satb = 2 * A * t + B;
            float satbtc = 2 * sa * Mathf.Sqrt(A * t * t + B * t + C);
            float sa23 = 8 * A * sa;

            float result = (satbtc * satb - (B * B - 4 * A * C) * Mathf.Log(satbtc + satb)) / sa23;
            return result;
        }
        public static float BezierLength(Vector2 vecA, Vector2 vecB, Vector2 vecC)
        {
            return BezierLength(1, vecA, vecB, vecC) - BezierLength(0, vecA, vecB, vecC);
        }

        #endregion

        #endregion

        #region Vector

        #region Basic

        #region Insert

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int InsertAsX(this Vector2Int vector, int x)
        {
            return new(x, vector.x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int InsertAsY(this Vector2Int vector, int y)
        {
            return new(vector.x, y, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int InsertAsZ(this Vector2Int vector, int z)
        {
            return new(vector.x, vector.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int InsertAs(this Vector2Int vector2, int extraNum, PlaneType planeType)
        {
            return planeType switch
            {
                PlaneType.XY => vector2.InsertAsZ(extraNum),
                PlaneType.XZ => vector2.InsertAsY(extraNum),
                PlaneType.YZ => vector2.InsertAsX(extraNum),
                _ => throw new ArgumentOutOfRangeException(nameof(planeType), planeType, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InsertAsX(this Vector2 vector, float x)
        {
            return new(x, vector.x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InsertAsY(this Vector2 vector, float y)
        {
            return new(vector.x, y, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InsertAsZ(this Vector2 vector, float z)
        {
            return new(vector.x, vector.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InsertAs(this Vector2 vector2, float extraNum, PlaneType planeType)
        {
            return planeType switch
            {
                PlaneType.XY => vector2.InsertAsZ(extraNum),
                PlaneType.XZ => vector2.InsertAsY(extraNum),
                PlaneType.YZ => vector2.InsertAsX(extraNum),
                _ => throw new ArgumentOutOfRangeException(nameof(planeType), planeType, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int As3DXY(this Vector2Int vector)
        {
            return vector.InsertAsZ(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int As3DXZ(this Vector2Int vector)
        {
            return vector.InsertAsY(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int As3DYZ(this Vector2Int vector)
        {
            return vector.InsertAsX(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 As3DXY(this Vector2 vector)
        {
            return vector.InsertAsZ(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 As3DXZ(this Vector2 vector)
        {
            return vector.InsertAsY(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 As3DYZ(this Vector2 vector)
        {
            return vector.InsertAsX(0);
        }

        #endregion

        #region Replace

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ReplaceX(this Vector4 vector, float x)
        {
            return new(x, vector.y, vector.z, vector.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ReplaceY(this Vector4 vector, float y)
        {
            return new(vector.x, y, vector.z, vector.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ReplaceZ(this Vector4 vector, float z)
        {
            return new(vector.x, vector.y, z, vector.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ReplaceW(this Vector4 vector, float w)
        {
            return new(vector.x, vector.y, vector.z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ReplaceRed(this Color color, float red)
        {
            return new(red, color.g, color.b, color.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ReplaceGreen(this Color color, float green)
        {
            return new(color.r, green, color.b, color.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ReplaceBlue(this Color color, float blue)
        {
            return new(color.r, color.g, blue, color.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ReplaceAlpha(this Color color, float alpha)
        {
            return new(color.r, color.g, color.b, alpha);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceX(this Vector3 vector, float x)
        {
            return new(x, vector.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceY(this Vector3 vector, float y)
        {
            return new(vector.x, y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceZ(this Vector3 vector, float z)
        {
            return new(vector.x, vector.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceX(this Vector3Int vector, int x)
        {
            return new(x, vector.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceY(this Vector3Int vector, int y)
        {
            return new(vector.x, y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceZ(this Vector3Int vector, int z)
        {
            return new(vector.x, vector.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceXY(this Vector3 vector, float x, float y)
        {
            return new(x, y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceYZ(this Vector3 vector, float y, float z)
        {
            return new(vector.x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceXZ(this Vector3 vector, float x, float z)
        {
            return new(x, vector.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceXY(this Vector3Int vector, int x, int y)
        {
            return new(x, y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceYZ(this Vector3Int vector, int y, int z)
        {
            return new(vector.x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceXZ(this Vector3Int vector, int x, int z)
        {
            return new(x, vector.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceXY(this Vector3 vector, Vector2 xy)
        {
            return new(xy.x, xy.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceYZ(this Vector3 vector, Vector2 yz)
        {
            return new(vector.x, yz.x, yz.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ReplaceXZ(this Vector3 vector, Vector2 xz)
        {
            return new(xz.x, vector.y, xz.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceXY(this Vector3Int vector, Vector2Int xy)
        {
            return new(xy.x, xy.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceYZ(this Vector3Int vector, Vector2Int yz)
        {
            return new(vector.x, yz.x, yz.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ReplaceXZ(this Vector3Int vector, Vector2Int xz)
        {
            return new(xz.x, vector.y, xz.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ReplaceX(this Vector2 vector, float x)
        {
            return new(x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ReplaceY(this Vector2 vector, float y)
        {
            return new(vector.x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ReplaceX(this Vector2Int vector, int x)
        {
            return new(x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ReplaceY(this Vector2Int vector, int y)
        {
            return new(vector.x, y);
        }

        #endregion

        #region Add

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddX(this Vector4 vector, float x)
        {
            return vector.ReplaceX(vector.x + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddY(this Vector4 vector, float y)
        {
            return vector.ReplaceY(vector.y + y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddZ(this Vector4 vector, float z)
        {
            return vector.ReplaceZ(vector.z + z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddW(this Vector4 vector, float w)
        {
            return vector.ReplaceW(vector.w + w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddRed(this Color color, float r)
        {
            return color.ReplaceRed(color.r + r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddGreen(this Color color, float g)
        {
            return color.ReplaceGreen(color.g + g);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddBlue(this Color color, float b)
        {
            return color.ReplaceBlue(color.b + b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddAlpha(this Color color, float a)
        {
            return color.ReplaceAlpha(color.a + a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int AddX(this Vector3Int vector, int x)
        {
            return vector.ReplaceX(vector.x + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int AddY(this Vector3Int vector, int y)
        {
            return vector.ReplaceY(vector.y + y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int AddZ(this Vector3Int vector, int z)
        {
            return vector.ReplaceZ(vector.z + z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int AddX(this Vector2Int vector, int x)
        {
            return vector.ReplaceX(vector.x + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int AddY(this Vector2Int vector, int y)
        {
            return vector.ReplaceY(vector.y + y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddX(this Vector3 vector, float x)
        {
            return vector.ReplaceX(vector.x + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddY(this Vector3 vector, float y)
        {
            return vector.ReplaceY(vector.y + y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddZ(this Vector3 vector, float z)
        {
            return vector.ReplaceZ(vector.z + z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AddX(this Vector2 vector, float x)
        {
            return vector.ReplaceX(vector.x + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AddY(this Vector2 vector, float y)
        {
            return vector.ReplaceY(vector.y + y);
        }

        #endregion

        #region Swap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapRed(this ref Color a, ref Color b)
        {
            (b.r, a.r) = (a.r, b.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapGreen(this ref Color a, ref Color b)
        {
            (b.g, a.g) = (a.g, b.g);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapBlue(this ref Color a, ref Color b)
        {
            (b.b, a.b) = (a.b, b.b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapAlpha(this ref Color a, ref Color b)
        {
            (b.a, a.a) = (a.a, b.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapX(this ref Vector4 a, ref Vector4 b)
        {
            (b.x, a.x) = (a.x, b.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapY(this ref Vector4 a, ref Vector4 b)
        {
            (b.y, a.y) = (a.y, b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapZ(this ref Vector4 a, ref Vector4 b)
        {
            (b.z, a.z) = (a.z, b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapW(this ref Vector4 a, ref Vector4 b)
        {
            (b.w, a.w) = (a.w, b.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapX(this ref Vector3Int a, ref Vector3Int b)
        {
            (b.x, a.x) = (a.x, b.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapY(this ref Vector3Int a, ref Vector3Int b)
        {
            (b.y, a.y) = (a.y, b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapZ(this ref Vector3Int a, ref Vector3Int b)
        {
            (b.z, a.z) = (a.z, b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapX(this ref Vector3 a, ref Vector3 b)
        {
            (b.x, a.x) = (a.x, b.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapY(this ref Vector3 a, ref Vector3 b)
        {
            (b.y, a.y) = (a.y, b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapZ(this ref Vector3 a, ref Vector3 b)
        {
            (b.z, a.z) = (a.z, b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapX(this ref Vector2 a, ref Vector2 b)
        {
            (b.x, a.x) = (a.x, b.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapY(this ref Vector2 a, ref Vector2 b)
        {
            (b.y, a.y) = (a.y, b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapX(this ref Vector2Int a, ref Vector2Int b)
        {
            (b.x, a.x) = (a.x, b.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapY(this ref Vector2Int a, ref Vector2Int b)
        {
            (b.y, a.y) = (a.y, b.y);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 SwapXY(this Vector2 a)
        {
            return new(a.y, a.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SwapXY(this Vector3 a)
        {
            return new(a.y, a.x, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SwapXZ(this Vector3 a)
        {
            return new(a.z, a.y, a.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SwapYZ(this Vector3 a)
        {
            return new(a.x, a.z, a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int SwapXY(this Vector2Int a)
        {
            return new(a.y, a.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int SwapXY(this Vector3Int a)
        {
            return new(a.y, a.x, a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int SwapXZ(this Vector3Int a)
        {
            return new(a.z, a.y, a.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int SwapYZ(this Vector3Int a)
        {
            return new(a.x, a.z, a.y);
        }

        #endregion

        #region Is NaN

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(this Vector2 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y);
        }

        #endregion

        #endregion

        #region Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 To3D(this Color color)
        {
            return new(color.r, color.g, color.b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 To4D(this Color color)
        {
            return color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 To3D(this Vector2 vector)
        {
            return (Vector3)vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int To3D(this Vector2Int vector)
        {
            return (Vector3Int)vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 To2D(this Vector3 vector)
        {
            return vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int To2D(this Vector3Int vector)
        {
            return (Vector2Int)vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 F(this Vector3Int vector)
        {
            return vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 F(this Vector2Int vector)
        {
            return vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 To2DF(this Vector3Int vector)
        {
            return vector.To2D();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 To3DF(this Vector2Int vector)
        {
            return vector.To3D();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int XZ(this Vector3Int vector)
        {
            return new(vector.x, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int XY(this Vector3Int vector)
        {
            return new(vector.x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int YZ(this Vector3Int vector)
        {
            return new(vector.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XZ(this Vector3 vector)
        {
            return new(vector.x, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XY(this Vector3 vector)
        {
            return new(vector.x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YZ(this Vector3 vector)
        {
            return new(vector.y, vector.z);
        }

        #endregion

        #region Math

        #region Angle To Direction

        /// <summary>
        /// 相对于Vector2.up逆时针旋转angle角度后的方向
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CounterclockwiseAngleToDirection(this float angle)
        {
            return Vector2.up.CounterclockwiseRotate(angle);
        }

        /// <summary>
        /// 相对于Vector2.up顺时针旋转angle角度后的方向
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClockwiseAngleToDirection(this float angle)
        {
            return Vector2.up.ClockwiseRotate(angle);
        }

        #endregion

        #region Angle & SignedAngle

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector2 from, Vector2 to)
        {
            return Vector2.Angle(from, to);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector2Int from, Vector2Int to)
        {
            return Vector2.Angle(from, to);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector3Int from, Vector3Int to)
        {
            return Vector3.Angle(from, to);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector4 from, Vector4 to)
        {
            float num = (float)Math.Sqrt(from.sqrMagnitude * (double)to.sqrMagnitude);
            if (num < 1.0000000036274937E-15)
            {
                return 0.0f;
            }

            return (float)Math.Acos(Mathf.Clamp(Vector4.Dot(from, to) / num, -1f, 1f)) * 57.29578f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Color from, Color to)
        {
            return Angle(from.To4D(), to.To4D());
        }

        /// <summary>
        /// 从from向量到to向量，逆时针为正，顺时针为负
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>[-180,180]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(this Vector2 from, Vector2 to)
        {
            return Vector2.SignedAngle(from, to);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(this Vector2Int from, Vector2Int to)
        {
            return Vector2.SignedAngle(from, to);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(this Vector3 from, Vector3 to, Vector3 axis)
        {
            return Vector3.SignedAngle(from, to, axis);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(this Vector3Int from, Vector3Int to, Vector3 axis)
        {
            return Vector3.SignedAngle(from, to, axis);
        }

        #endregion

        #region ClockwiseAngle

        /// <summary>
        /// 相对于(0,1)的顺时针角度
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClockwiseAngle(this Vector2 vector)
        {
            return Vector2.up.ClockwiseAngle(vector);
        }

        /// <summary>
        /// 顺时针角度
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>[0,360)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClockwiseAngle(this Vector2 from, Vector2 to)
        {
            var signedAngle = from.SignedAngle(to);

            if (signedAngle > 0)
            {
                return 360 - signedAngle;
            }
            
            return -signedAngle;
        }

        /// <summary>
        /// 逆时针角度
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>[0,360)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CounterclockwiseAngle(this Vector2 from, Vector2 to)
        {
            var signedAngle = from.SignedAngle(to);

            if (signedAngle < 0)
            {
                return 360 + signedAngle;
            }

            return signedAngle;
        }

        #endregion

        #region Rotate

        /// <summary>
        /// 绕原点以指定角度（以度为单位）逆时针旋转一个二维向量。
        /// </summary>
        /// <param name="origin">要旋转的原始向量</param>
        /// <param name="angle">旋转的角度</param>
        /// <returns>旋转后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CounterclockwiseRotate(this Vector2 origin, float angle)
        {
            var rotation = Quaternion.Euler(0f, 0f, angle);
            return rotation * origin;
        }

        /// <summary>
        /// 绕原点以指定角度（以度为单位）顺时针旋转一个二维向量。
        /// </summary>
        /// <param name="origin">要旋转的原始向量</param>
        /// <param name="angle">旋转的角度</param>
        /// <returns>旋转后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClockwiseRotate(this Vector2 origin, float angle)
        {
            var rotation = Quaternion.Euler(0f, 0f, -angle);
            return rotation * origin;
        }


        #endregion

        #endregion

        #region Geometry

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> GetRangeOfPoints(this int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                yield return i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> GetCircularRangeOfPoints(this int start, int end, int count)
        {
            int result = start;
            for (int i = 0; i < count; i++)
            {
                yield return result;
                result++;
                if (result > end)
                {
                    result = start;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> GetSteppedPoints(this int start, int end, int step = 1)
        {
            int count = start;
            while (count <= end)
            {
                yield return count;

                count += step;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<float> GetSteppedPoints(this float start, float end, float step = 1)
        {
            float count = start;
            while (count <= end)
            {
                yield return count;

                count += step;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector2Int> GetRectangleOfPoints(this Vector2Int start, Vector2Int end)
        {
            if (end.AnyNumberBelow(start))
            {
                yield break;
            }
            else
            {
                for (int i = start.x; i <= end.x; i++)
                {
                    for (int j = start.y; j <= end.y; j++)
                    {
                        yield return new(i, j);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector2Int> GetRectangleOfPoints(this Vector2Int size)
        {
            return GetRectangleOfPoints(Vector2Int.zero, size - Vector2Int.one);
        }

        #region Cube

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetCubeOfPoints(this Vector3Int start, Vector3Int end)
        {
            if (end.AnyNumberBelow(start))
            {
                yield break;
            }
            else
            {
                for (int i = start.x; i <= end.x; i++)
                {
                    for (int j = start.y; j <= end.y; j++)
                    {
                        for (int k = start.z; k <= end.z; k++)
                        {
                            yield return new(i, j, k);
                        }

                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetCubeOfPoints(this Vector3Int size)
        {
            return GetCubeOfPoints(Vector3Int.zero, size - Vector3Int.one);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetFacePointsOfCube(this Vector3Int start, Vector3Int end, FaceType faceType)
        {
            if (faceType.HasFlag(FaceType.Right))
            {
                for (int y = start.y; y <= end.y; y++)
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(end.x, y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Left))
            {
                for (int y = start.y; y <= end.y; y++)
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(start.x, y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Up))
            {
                for (int x = start.x; x <= end.x; x++)
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(x, end.y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Down))
            {
                for (int x = start.x; x <= end.x; x++)
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(x, start.y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Forward))
            {
                for (int x = start.x; x <= end.x; x++)
                {
                    for (int y = start.y; y <= end.y; y++)
                    {
                        yield return new(x, y, end.z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Back))
            {
                for (int x = start.x; x <= end.x; x++)
                {
                    for (int y = start.y; y <= end.y; y++)
                    {
                        yield return new(x, y, start.z);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetFacePointsOfCube(this Vector3Int size, FaceType faceType)
        {
            return GetFacePointsOfCube(Vector3Int.zero, size - Vector3Int.one, faceType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetInnerFacePointsOfCube(this Vector3Int start, Vector3Int end, FaceType faceType)
        {
            if (faceType.HasFlag(FaceType.Right))
            {
                for (int y = start.y + 1; y < end.y; y++)
                {
                    for (int z = start.z + 1; z < end.z; z++)
                    {
                        yield return new(end.x, y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Left))
            {
                for (int y = start.y + 1; y < end.y; y++)
                {
                    for (int z = start.z + 1; z < end.z; z++)
                    {
                        yield return new(start.x, y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Up))
            {
                for (int x = start.x + 1; x < end.x; x++)
                {
                    for (int z = start.z + 1; z < end.z; z++)
                    {
                        yield return new(x, end.y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Down))
            {
                for (int x = start.x + 1; x < end.x; x++)
                {
                    for (int z = start.z + 1; z < end.z; z++)
                    {
                        yield return new(x, start.y, z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Forward))
            {
                for (int x = start.x + 1; x < end.x; x++)
                {
                    for (int y = start.y + 1; y < end.y; y++)
                    {
                        yield return new(x, y, end.z);
                    }
                }
            }

            if (faceType.HasFlag(FaceType.Back))
            {
                for (int x = start.x + 1; x < end.x; x++)
                {
                    for (int y = start.y + 1; y < end.y; y++)
                    {
                        yield return new(x, y, start.z);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetInnerFacePointsOfCube(this Vector3Int size, FaceType faceType)
        {
            return GetInnerFacePointsOfCube(Vector3Int.zero, size - Vector3Int.one, faceType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetAllInnerFacePointsOfCube(this Vector3Int start, Vector3Int end)
        {
            for (int y = start.y + 1; y < end.y; y++)
            {
                for (int z = start.z + 1; z < end.z; z++)
                {
                    yield return new(end.x, y, z);
                }
            }

            for (int y = start.y + 1; y < end.y; y++)
            {
                for (int z = start.z + 1; z < end.z; z++)
                {
                    yield return new(start.x, y, z);
                }
            }

            for (int x = start.x + 1; x < end.x; x++)
            {
                for (int z = start.z + 1; z < end.z; z++)
                {
                    yield return new(x, end.y, z);
                }
            }

            for (int x = start.x + 1; x < end.x; x++)
            {
                for (int z = start.z + 1; z < end.z; z++)
                {
                    yield return new(x, start.y, z);
                }
            }

            for (int x = start.x + 1; x < end.x; x++)
            {
                for (int y = start.y + 1; y < end.y; y++)
                {
                    yield return new(x, y, end.z);
                }
            }

            for (int x = start.x + 1; x < end.x; x++)
            {
                for (int y = start.y + 1; y < end.y; y++)
                {
                    yield return new(x, y, start.z);
                }
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetAllInnerFacePointsOfCube(this Vector3Int size)
        {
            return GetAllInnerFacePointsOfCube(Vector3Int.zero, size - Vector3Int.one);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetAllFacePointsOfCube(this Vector3Int start, Vector3Int end)
        {
            return GetAllInnerFacePointsOfCube(start, end).Concat(GetAllEdgePointsOfCube(start, end));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetAllFacePointsOfCube(this Vector3Int size)
        {
            return GetAllInnerFacePointsOfCube(size).Concat(GetAllEdgePointsOfCube(size));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetInnerPointsOfCube(this Vector3Int start, Vector3Int end)
        {
            for (int x = start.x + 1; x < end.x; x++)
            {
                for (int y = start.y + 1; y < end.y; y++)
                {
                    for (int z = start.z + 1; z < end.z; z++)
                    {
                        yield return new(x, y, z);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetInnerPointsOfCube(this Vector3Int size)
        {
            return GetInnerPointsOfCube(Vector3Int.zero, size - Vector3Int.one);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetAllEdgePointsOfCube(this Vector3Int start, Vector3Int end)
        {
            if (start.x != end.x)
            {
                if (start.y != end.y)
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(start.x, start.y, z);
                        yield return new(start.x, end.y, z);
                        yield return new(end.x, start.y, z);
                        yield return new(end.x, end.y, z);
                    }

                    for (int x = start.x + 1; x < end.x; x++)
                    {
                        yield return new(x, start.y, start.z);
                        yield return new(x, start.y, end.z);
                        yield return new(x, end.y, start.z);
                        yield return new(x, end.y, end.z);
                    }

                    for (int y = start.y + 1; y < end.y; y++)
                    {
                        yield return new(start.x, y, start.z);
                        yield return new(start.x, y, end.z);
                        yield return new(end.x, y, start.z);
                        yield return new(end.x, y, end.z);
                    }
                }
                else
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(start.x, start.y, z);
                        yield return new(end.x, start.y, z);
                    }

                    for (int x = start.x + 1; x < end.x; x++)
                    {
                        yield return new(x, start.y, start.z);
                        yield return new(x, start.y, end.z);
                    }
                }
            }
            else
            {
                if (start.y != end.y)
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(start.x, start.y, z);
                        yield return new(start.x, end.y, z);
                    }

                    for (int y = start.y + 1; y < end.y; y++)
                    {
                        yield return new(start.x, y, start.z);
                        yield return new(start.x, y, end.z);
                    }
                }
                else
                {
                    for (int z = start.z; z <= end.z; z++)
                    {
                        yield return new(start.x, start.y, z);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GetAllEdgePointsOfCube(this Vector3Int size)
        {
            return GetAllEdgePointsOfCube(Vector3Int.zero, size - Vector3Int.one);
        }

        #endregion

        #region Circle

        public static IEnumerable<Vector3Int> GetCircleOfPoints(this Vector3Int pivot, float radius, DistanceType distanceType = DistanceType.Manhattan, PlaneType planeType = PlaneType.XY)
        {
            return distanceType switch
            {
                DistanceType.Manhattan => pivot.GetManhattanCircleOfPoints(radius, planeType),
                DistanceType.Euclidean => pivot.GetEuclideanCircleOfPoints(radius, planeType),
                _ => throw new InvalidEnumArgumentException(nameof(distanceType))
            };
        }

        public static IEnumerable<Vector3Int> GetManhattanCircleOfPoints(this Vector3Int pivot, float radius, PlaneType planeType = PlaneType.XY)
        {
            switch (planeType)
            {
                case PlaneType.XY:
                    foreach (var pos in GetManhattanCircleOfPoints(new(pivot.x, pivot.y), radius))
                    {
                        yield return new(pos.x, pos.y, pivot.z);
                    }
                    break;
                case PlaneType.XZ:
                    foreach (var pos in GetManhattanCircleOfPoints(new(pivot.x, pivot.z), radius))
                    {
                        yield return new(pos.x, pivot.y, pos.y);
                    }
                    break;
                case PlaneType.YZ:
                    foreach (var pos in GetManhattanCircleOfPoints(new(pivot.y, pivot.z), radius))
                    {
                        yield return new(pivot.x, pos.x, pos.y);
                    }
                    break;
            }
        }

        public static IEnumerable<Vector2Int> GetManhattanCircleOfPoints(this Vector2Int pivot, float radius)
        {
            if (radius < 0)
            {
                yield break;
            }

            int r = Mathf.FloorToInt(radius);

            for (int l = 0; l < r; l++)
            {
                int py = pivot.y + (r - l);
                int ny = pivot.y - (r - l);
                for (int x = pivot.x - l; x <= pivot.x + l; x++)
                {
                    yield return new(x, py);
                    yield return new(x, ny);
                }
            }

            for (int x = pivot.x - r; x <= pivot.x + r; x++)
            {
                yield return new(x, pivot.y);
            }
        }

        public static IEnumerable<Vector3Int> GetEuclideanCircleOfPoints(this Vector3Int pivot, float radius, PlaneType planeType = PlaneType.XY)
        {
            switch (planeType)
            {
                case PlaneType.XY:
                    foreach (var pos in GetEuclideanCircleOfPoints(new(pivot.x, pivot.y), radius))
                    {
                        yield return new(pos.x, pos.y, pivot.z);
                    }
                    break;
                case PlaneType.XZ:
                    foreach (var pos in GetEuclideanCircleOfPoints(new(pivot.x, pivot.z), radius))
                    {
                        yield return new(pos.x, pivot.y, pos.y);
                    }
                    break;
                case PlaneType.YZ:
                    foreach (var pos in GetEuclideanCircleOfPoints(new(pivot.y, pivot.z), radius))
                    {
                        yield return new(pivot.x, pos.x, pos.y);
                    }
                    break;
            }
        }

        public static IEnumerable<Vector2Int> GetEuclideanCircleOfPoints(this Vector2Int pivot, float radius)
        {
            if (radius < 0)
            {
                yield break;
            }

            foreach (var pos in GetManhattanCircleOfPoints(pivot, radius))
            {
                yield return pos;
            }

            int r = Mathf.FloorToInt(radius);

            List<Vector2Int> quarterCircle = new();

            for (int l = 0; l < r; l++)
            {
                int y = pivot.y + l + 1;
                int xMax = pivot.x + r;
                for (int x = pivot.x + r - l; x <= xMax; x++)
                {
                    Vector2Int newPos = new(x, y);
                    if (newPos.EuclideanDistance(pivot) <= radius)
                    {
                        quarterCircle.Add(newPos);
                    }
                }
            }

            foreach (var quarterPos in quarterCircle)
            {
                foreach (var pos in quarterPos.Symmetric(pivot))
                {
                    yield return pos;
                }
            }
        }

        #endregion

        #endregion

        #region Traversal

        #region Color

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ForeachNumber(this Color a, Color b, Color c, Func<float, float, float, float> func)
        {
            return new(func(a.r, b.r, c.r), func(a.g, b.g, c.g), func(a.b, b.b, c.b), func(a.a, b.a, c.a));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ForeachNumber(this Color a, Color b, Func<float, float, float> func)
        {
            return new(func(a.r, b.r), func(a.g, b.g), func(a.b, b.b), func(a.a, b.a));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ForeachNumber(this Color a, float b, Func<float, float, float> func)
        {
            return new(func(a.r, b), func(a.g, b), func(a.b, b), func(a.a, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ForeachNumber(this Color a, Func<float, float> func)
        {
            return new(func(a.r), func(a.g), func(a.b), func(a.a));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForeachNumber(this Color a, Action<float> func)
        {
            func(a.r);
            func(a.g);
            func(a.b);
            func(a.a);
        }

        #endregion

        #region Vector4

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ForeachNumber(this Vector4 a, Vector4 b, Vector4 c, Func<float, float, float, float> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y), func(a.z, b.z, c.z), func(a.w, b.w, c.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ForeachNumber(this Vector4 a, Vector4 b, Func<float, float, float> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y), func(a.z, b.z), func(a.w, b.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ForeachNumber(this Vector4 a, float b, Func<float, float, float> func)
        {
            return new(func(a.x, b), func(a.y, b), func(a.z, b), func(a.w, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ForeachNumber(this Vector4 a, Func<float, float> func)
        {
            return new(func(a.x), func(a.y), func(a.z), func(a.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForeachNumber(this Vector4 a, Action<float> func)
        {
            func(a.x);
            func(a.y);
            func(a.z);
            func(a.w);
        }

        #endregion

        #region Vector3Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3Int a, Vector3Int b, Vector3Int c, Func<int, int, int, int> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y), func(a.z, b.z, c.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3Int a, Vector3Int b, Vector3Int c, Func<int, int, int, float> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y), func(a.z, b.z, c.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3Int a, Vector3Int b, Func<int, int, int> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y), func(a.z, b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3Int a, Vector3Int b, Func<int, int, float> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y), func(a.z, b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3Int a, int b, Func<int, int, int> func)
        {
            return new(func(a.x, b), func(a.y, b), func(a.z, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3Int a, int b, Func<int, int, float> func)
        {
            return new(func(a.x, b), func(a.y, b), func(a.z, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3Int a, Func<int, int> func)
        {
            return new(func(a.x), func(a.y), func(a.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3Int a, Func<int, float> func)
        {
            return new(func(a.x), func(a.y), func(a.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForeachNumber(this Vector3Int a, Action<int> func)
        {
            func(a.x);
            func(a.y);
            func(a.z);
        }

        #endregion

        #region Vector3

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3 a, Vector3 b, Vector3 c, Func<float, float, float, float> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y), func(a.z, b.z, c.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3 a, Vector3 b, Vector3 c, Func<float, float, float, int> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y), func(a.z, b.z, c.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3 a, Vector3 b, Func<float, float, float> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y), func(a.z, b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3 a, Vector3 b, Func<float, float, int> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y), func(a.z, b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3 a, float b, Func<float, float, float> func)
        {
            return new(func(a.x, b), func(a.y, b), func(a.z, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3 a, float b, Func<float, float, int> func)
        {
            return new(func(a.x, b), func(a.y, b), func(a.z, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ForeachNumber(this Vector3 a, Func<float, float> func)
        {
            return new(func(a.x), func(a.y), func(a.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ForeachNumber(this Vector3 a, Func<float, int> func)
        {
            return new(func(a.x), func(a.y), func(a.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForeachNumber(this Vector3 a, Action<float> func)
        {
            func(a.x);
            func(a.y);
            func(a.z);
        }

        #endregion

        #region Vector2Int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2Int a, Vector2Int b, Vector2Int c, Func<int, int, int, int> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2Int a, Vector2Int b, Vector2Int c, Func<int, int, int, float> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2Int a, Vector2Int b, Func<int, int, int> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2Int a, Vector2Int b, Func<int, int, float> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2Int a, int b, Func<int, int, int> func)
        {
            return new(func(a.x, b), func(a.y, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2Int a, int b, Func<int, int, float> func)
        {
            return new(func(a.x, b), func(a.y, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2Int a, Func<int, int> func)
        {
            return new(func(a.x), func(a.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2Int a, Func<int, float> func)
        {
            return new(func(a.x), func(a.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForeachNumber(this Vector2Int a, Action<int> func)
        {
            func(a.x);
            func(a.y);
        }

        #endregion

        #region Vector2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2 a, Vector2 b, Vector2 c, Func<float, float, float, float> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2 a, Vector2 b, Vector2 c, Func<float, float, float, int> func)
        {
            return new(func(a.x, b.x, c.x), func(a.y, b.y, c.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2 a, Vector2 b, Func<float, float, float> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2 a, Vector2 b, Func<float, float, int> func)
        {
            return new(func(a.x, b.x), func(a.y, b.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2 a, float b, Func<float, float, float> func)
        {
            return new(func(a.x, b), func(a.y, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2 a, float b, Func<float, float, int> func)
        {
            return new(func(a.x, b), func(a.y, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ForeachNumber(this Vector2 a, Func<float, float> func)
        {
            return new(func(a.x), func(a.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ForeachNumber(this Vector2 a, Func<float, int> func)
        {
            return new(func(a.x), func(a.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForeachNumber(this Vector2 a, Action<float> func)
        {
            func(a.x);
            func(a.y);
        }

        #endregion

        #endregion

        #region Bool

        #region CompareAll

        #region AllBelowOrEqual

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Color a, float comparison)
        {
            return a.All(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector4 a, float comparison)
        {
            return a.All(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector3Int a, int comparison)
        {
            return a.All(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector3 a, float comparison)
        {
            return a.All(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector2Int a, int comparison)
        {
            return a.All(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector2 a, float comparison)
        {
            return a.All(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Color a, Color comparison)
        {
            return a.All(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector4 a, Vector4 comparison)
        {
            return a.All(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector3Int a, Vector3Int comparison)
        {
            return a.All(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector3 a, Vector3 comparison)
        {
            return a.All(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector2Int a, Vector2Int comparison)
        {
            return a.All(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this Vector2 a, Vector2 comparison)
        {
            return a.All(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this double a, double comparison)
        {
            return a <= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this float a, float comparison)
        {
            return a <= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual(this int a, int comparison)
        {
            return a <= comparison;
        }

        #endregion

        #region AllBelow

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Color a, float comparison)
        {
            return a.All(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector4 a, float comparison)
        {
            return a.All(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector3Int a, int comparison)
        {
            return a.All(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector3 a, float comparison)
        {
            return a.All(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector2Int a, int comparison)
        {
            return a.All(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector2 a, float comparison)
        {
            return a.All(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Color a, Color comparison)
        {
            return a.All(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector4 a, Vector4 comparison)
        {
            return a.All(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector3Int a, Vector3Int comparison)
        {
            return a.All(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector3 a, Vector3 comparison)
        {
            return a.All(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector2Int a, Vector2Int comparison)
        {
            return a.All(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this Vector2 a, Vector2 comparison)
        {
            return a.All(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this double a, double comparison)
        {
            return a < comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this float a, float comparison)
        {
            return a < comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow(this int a, int comparison)
        {
            return a < comparison;
        }

        #endregion

        #region AllAboveOrEqual

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Color a, float comparison)
        {
            return a.All(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector4 a, float comparison)
        {
            return a.All(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector3Int a, int comparison)
        {
            return a.All(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector3 a, float comparison)
        {
            return a.All(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector2Int a, int comparison)
        {
            return a.All(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector2 a, int comparison)
        {
            return a.All(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Color a, Color comparison)
        {
            return a.All(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector4 a, Vector4 comparison)
        {
            return a.All(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector3Int a, Vector3Int comparison)
        {
            return a.All(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector3 a, Vector3 comparison)
        {
            return a.All(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector2Int a, Vector2Int comparison)
        {
            return a.All(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this Vector2 a, Vector2 comparison)
        {
            return a.All(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this double a, double comparison)
        {
            return a >= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this float a, float comparison)
        {
            return a >= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual(this int a, int comparison)
        {
            return a >= comparison;
        }

        #endregion

        #region AllAbove

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Color a, float comparison)
        {
            return a.All(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector4 a, float comparison)
        {
            return a.All(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector3Int a, int comparison)
        {
            return a.All(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector3 a, float comparison)
        {
            return a.All(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector2Int a, int comparison)
        {
            return a.All(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector2 a, int comparison)
        {
            return a.All(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Color a, Color comparison)
        {
            return a.All(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector4 a, Vector4 comparison)
        {
            return a.All(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector3Int a, Vector3Int comparison)
        {
            return a.All(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector3 a, Vector3 comparison)
        {
            return a.All(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector2Int a, Vector2Int comparison)
        {
            return a.All(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this Vector2 a, Vector2 comparison)
        {
            return a.All(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this double a, double comparison)
        {
            return a > comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this float a, float comparison)
        {
            return a > comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove(this int a, int comparison)
        {
            return a > comparison;
        }

        #endregion

        #endregion

        #region CompareAny

        #region AnyBelowOrEqual

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Color a, float comparison)
        {
            return a.Any(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector4 a, float comparison)
        {
            return a.Any(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector3Int a, int comparison)
        {
            return a.Any(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector3 a, float comparison)
        {
            return a.Any(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector2Int a, int comparison)
        {
            return a.Any(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector2 a, float comparison)
        {
            return a.Any(num => num <= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Color a, Color comparison)
        {
            return a.Any(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector4 a, Vector4 comparison)
        {
            return a.Any(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector3Int a, Vector3Int comparison)
        {
            return a.Any(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector3 a, Vector3 comparison)
        {
            return a.Any(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector2Int a, Vector2Int comparison)
        {
            return a.Any(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this Vector2 a, Vector2 comparison)
        {
            return a.Any(comparison, (a, b) => a <= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this double a, double comparison)
        {
            return a <= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this float a, float comparison)
        {
            return a <= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual(this int a, int comparison)
        {
            return a <= comparison;
        }

        #endregion

        #region AnyBelow

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Color a, float comparison)
        {
            return a.Any(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector4 a, float comparison)
        {
            return a.Any(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector3Int a, int comparison)
        {
            return a.Any(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector3 a, float comparison)
        {
            return a.Any(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector2Int a, int comparison)
        {
            return a.Any(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector2 a, float comparison)
        {
            return a.Any(num => num < comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Color a, Color comparison)
        {
            return a.Any(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector4 a, Vector4 comparison)
        {
            return a.Any(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector3Int a, Vector3Int comparison)
        {
            return a.Any(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector3 a, Vector3 comparison)
        {
            return a.Any(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector2Int a, Vector2Int comparison)
        {
            return a.Any(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this Vector2 a, Vector2 comparison)
        {
            return a.Any(comparison, (a, b) => a < b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this double a, double comparison)
        {
            return a < comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this float a, float comparison)
        {
            return a < comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow(this int a, int comparison)
        {
            return a < comparison;
        }

        #endregion

        #region AnyAboveOrEqual

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Color a, float comparison)
        {
            return a.Any(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector4 a, float comparison)
        {
            return a.Any(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector3Int a, int comparison)
        {
            return a.Any(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector3 a, float comparison)
        {
            return a.Any(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector2Int a, int comparison)
        {
            return a.Any(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector2 a, float comparison)
        {
            return a.Any(num => num >= comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Color a, Color comparison)
        {
            return a.Any(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector4 a, Vector4 comparison)
        {
            return a.Any(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector3Int a, Vector3Int comparison)
        {
            return a.Any(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector3 a, Vector3 comparison)
        {
            return a.Any(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector2Int a, Vector2Int comparison)
        {
            return a.Any(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this Vector2 a, Vector2 comparison)
        {
            return a.Any(comparison, (a, b) => a >= b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this double a, double comparison)
        {
            return a >= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this float a, float comparison)
        {
            return a >= comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual(this int a, int comparison)
        {
            return a >= comparison;
        }

        #endregion

        #region AnyAbove

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Color a, float comparison)
        {
            return a.Any(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector4 a, float comparison)
        {
            return a.Any(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector3Int a, int comparison)
        {
            return a.Any(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector3 a, float comparison)
        {
            return a.Any(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector2Int a, int comparison)
        {
            return a.Any(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector2 a, float comparison)
        {
            return a.Any(num => num > comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Color a, Color comparison)
        {
            return a.Any(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector4 a, Vector4 comparison)
        {
            return a.Any(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector3Int a, Vector3Int comparison)
        {
            return a.Any(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector3 a, Vector3 comparison)
        {
            return a.Any(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector2Int a, Vector2Int comparison)
        {
            return a.Any(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this Vector2 a, Vector2 comparison)
        {
            return a.Any(comparison, (a, b) => a > b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this double a, double comparison)
        {
            return a > comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this float a, float comparison)
        {
            return a > comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove(this int a, int comparison)
        {
            return a > comparison;
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}

