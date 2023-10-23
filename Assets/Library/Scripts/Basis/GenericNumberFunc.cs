using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Basis;
using UnityEngine;

namespace GenericBasis
{
    public static class GenericNumberFunc
    {
        public static HashSet<Type> numberTypes = new()
    {
        typeof(int),
        typeof(float),
        typeof(double)
    };

        public static HashSet<Type> vectorTypes = new()
    {
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector2Int),
        typeof(Vector3Int),
        typeof(Vector4),
        typeof(Color)
    };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this Type type) => numberTypes.Contains(type);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVector(this Type type) => vectorTypes.Contains(type);

        public static T ConvertNumber<T>(object value) => (T)Convert.ChangeType(value, typeof(T));

        #region Clamp

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ClampMin<T>(this T a, T min) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.ClampMin(a.ConvertTo<int>(), min.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.ClampMin(a.ConvertTo<float>(), min.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.ClampMin(a.ConvertTo<double>(), min.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.ClampMin(a.ConvertTo<Vector2>(), min.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.ClampMin(a.ConvertTo<Vector3>(), min.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.ClampMin(a.ConvertTo<Vector4>(), min.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.ClampMin(a.ConvertTo<Vector2Int>(), min.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.ClampMin(a.ConvertTo<Vector3Int>(), min.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => MathFunc.ClampMin(a.ConvertTo<Color>(), min.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ClampMax<T>(this T a, T min) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.ClampMax(a.ConvertTo<int>(), min.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.ClampMax(a.ConvertTo<float>(), min.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.ClampMax(a.ConvertTo<double>(), min.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.ClampMax(a.ConvertTo<Vector2>(), min.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.ClampMax(a.ConvertTo<Vector3>(), min.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.ClampMax(a.ConvertTo<Vector4>(), min.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.ClampMax(a.ConvertTo<Vector2Int>(), min.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.ClampMax(a.ConvertTo<Vector3Int>(), min.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => MathFunc.ClampMax(a.ConvertTo<Color>(), min.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Clamp<T>(this T a, T min, T max) where T : IEquatable<T>
        {
            return a.ClampMin(min).ClampMax(max);
        }

        #endregion

        #region Compare

        #region AllNumber

        /// <summary>
        /// return true if num is in range [min, max]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBetweenInclusive<T>(this T num, T min, T max) where T : IEquatable<T>
        {
            return num.AllNumberAboveOrEqual(min) && num.AllNumberBelowOrEqual(max);
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
        public static bool AllNumberBetweenExclusive<T>(this T num, T min, T max) where T : IEquatable<T>
        {
            return num.AllNumberAbove(min) && num.AllNumberBelow(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelow<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AllNumberBelow(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AllNumberBelow(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AllNumberBelow(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AllNumberBelow(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AllNumberBelow(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AllNumberBelow(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AllNumberBelow(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AllNumberBelow(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AllNumberBelow(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAbove<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AllNumberAbove(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AllNumberAbove(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AllNumberAbove(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AllNumberAbove(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AllNumberAbove(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AllNumberAbove(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AllNumberAbove(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AllNumberAbove(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AllNumberAbove(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberBelowOrEqual<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AllNumberBelowOrEqual(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllNumberAboveOrEqual<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AllNumberAboveOrEqual(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        #endregion

        #region AnyNumber

        /// <summary>
        /// return true if num is in range [min, max]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBetweenInclusive<T>(this T num, T min, T max) where T : IEquatable<T>
        {
            return num.AnyNumberAboveOrEqual(min) && num.AnyNumberBelowOrEqual(max);
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
        public static bool AnyNumberBetweenExclusive<T>(this T num, T min, T max) where T : IEquatable<T>
        {
            return num.AnyNumberAbove(min) && num.AnyNumberBelow(max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberBelow(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AnyNumberBelow(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AnyNumberBelow(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AnyNumberBelow(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AnyNumberBelow(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AnyNumberBelow(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AnyNumberBelow(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AnyNumberBelow(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AnyNumberBelow(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberAbove(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AnyNumberAbove(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AnyNumberAbove(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AnyNumberAbove(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AnyNumberAbove(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AnyNumberAbove(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AnyNumberAbove(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AnyNumberAbove(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AnyNumberAbove(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual<T>(this T a, T comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<int>(), comparison.ConvertTo<int>()),
                float => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<float>(), comparison.ConvertTo<float>()),
                double => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<double>(), comparison.ConvertTo<double>()),
                Vector2 => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector2>(), comparison.ConvertTo<Vector2>()),
                Vector3 => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector3>(), comparison.ConvertTo<Vector3>()),
                Vector4 => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector4>(), comparison.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector2Int>(), comparison.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector3Int>(), comparison.ConvertTo<Vector3Int>()),
                Color => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Color>(), comparison.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelowOrEqual<T>(this T a, double comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<int>(), MathFunc.Floor(comparison)),
                float => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<float>(), comparison.F()),
                double => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<double>(), comparison),
                Vector2 => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector2>(), comparison.F()),
                Vector3 => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector3>(), comparison.F()),
                Vector4 => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector4>(), comparison.F()),
                Vector2Int => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector2Int>(), MathFunc.Floor(comparison)),
                Vector3Int => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Vector3Int>(), MathFunc.Floor(comparison)),
                Color => MathFunc.AnyNumberBelowOrEqual(a.ConvertTo<Color>(), comparison.F()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberBelow<T>(this T a, double comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberBelow(a.ConvertTo<int>(), MathFunc.Floor(comparison)),
                float => MathFunc.AnyNumberBelow(a.ConvertTo<float>(), comparison.F()),
                double => MathFunc.AnyNumberBelow(a.ConvertTo<double>(), comparison),
                Vector2 => MathFunc.AnyNumberBelow(a.ConvertTo<Vector2>(), comparison.F()),
                Vector3 => MathFunc.AnyNumberBelow(a.ConvertTo<Vector3>(), comparison.F()),
                Vector4 => MathFunc.AnyNumberBelow(a.ConvertTo<Vector4>(), comparison.F()),
                Vector2Int => MathFunc.AnyNumberBelow(a.ConvertTo<Vector2Int>(), MathFunc.Floor(comparison)),
                Vector3Int => MathFunc.AnyNumberBelow(a.ConvertTo<Vector3Int>(), MathFunc.Floor(comparison)),
                Color => MathFunc.AnyNumberBelow(a.ConvertTo<Color>(), comparison.F()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAboveOrEqual<T>(this T a, double comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<int>(), MathFunc.Ceiling(comparison)),
                float => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<float>(), comparison.F()),
                double => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<double>(), comparison),
                Vector2 => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector2>(), comparison.F()),
                Vector3 => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector3>(), comparison.F()),
                Vector4 => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector4>(), comparison.F()),
                Vector2Int => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector2Int>(), MathFunc.Ceiling(comparison)),
                Vector3Int => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Vector3Int>(), MathFunc.Ceiling(comparison)),
                Color => MathFunc.AnyNumberAboveOrEqual(a.ConvertTo<Color>(), comparison.F()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyNumberAbove<T>(this T a, double comparison) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.AnyNumberAbove(a.ConvertTo<int>(), MathFunc.Ceiling(comparison)),
                float => MathFunc.AnyNumberAbove(a.ConvertTo<float>(), comparison.F()),
                double => MathFunc.AnyNumberAbove(a.ConvertTo<double>(), comparison),
                Vector2 => MathFunc.AnyNumberAbove(a.ConvertTo<Vector2>(), comparison.F()),
                Vector3 => MathFunc.AnyNumberAbove(a.ConvertTo<Vector3>(), comparison.F()),
                Vector4 => MathFunc.AnyNumberAbove(a.ConvertTo<Vector4>(), comparison.F()),
                Vector2Int => MathFunc.AnyNumberAbove(a.ConvertTo<Vector2Int>(), MathFunc.Ceiling(comparison)),
                Vector3Int => MathFunc.AnyNumberAbove(a.ConvertTo<Vector3Int>(), MathFunc.Ceiling(comparison)),
                Color => MathFunc.AnyNumberAbove(a.ConvertTo<Color>(), comparison.F()),
                _ => throw new ArgumentException()
            };
        }


        #endregion

        #endregion

        #region Round

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Round<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => a.ConvertTo<T>(),
                float => MathFunc.Round(a.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Round(a.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Round(a.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Round(a.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Round(a.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => a.ConvertTo<T>(),
                Vector3Int => a.ConvertTo<T>(),
                Color => a.ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Ceiling<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => a.ConvertTo<T>(),
                float => MathFunc.Ceiling(a.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Ceiling(a.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Ceiling(a.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Ceiling(a.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Ceiling(a.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => a.ConvertTo<T>(),
                Vector3Int => a.ConvertTo<T>(),
                Color => a.ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Floor<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => a.ConvertTo<T>(),
                float => MathFunc.Floor(a.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Floor(a.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Floor(a.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Floor(a.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Floor(a.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => a.ConvertTo<T>(),
                Vector3Int => a.ConvertTo<T>(),
                Color => a.ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        #endregion

        #region + - * / ^

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Add<T>(this T a, T b) where T : IEquatable<T>
        {
            return a switch
            {
                int => (a.ConvertTo<int>() + b.ConvertTo<int>()).ConvertTo<T>(),
                float => (a.ConvertTo<float>() + b.ConvertTo<float>()).ConvertTo<T>(),
                double => (a.ConvertTo<double>() + b.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => (a.ConvertTo<Vector2>() + b.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => (a.ConvertTo<Vector3>() + b.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => (a.ConvertTo<Vector4>() + b.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => (a.ConvertTo<Vector2Int>() + b.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => (a.ConvertTo<Vector3Int>() + b.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => (a.ConvertTo<Color>() + b.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Subtract<T>(this T a, T b) where T : IEquatable<T>
        {
            return a switch
            {
                int => (a.ConvertTo<int>() - b.ConvertTo<int>()).ConvertTo<T>(),
                float => (a.ConvertTo<float>() - b.ConvertTo<float>()).ConvertTo<T>(),
                double => (a.ConvertTo<double>() - b.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => (a.ConvertTo<Vector2>() - b.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => (a.ConvertTo<Vector3>() - b.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => (a.ConvertTo<Vector4>() - b.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => (a.ConvertTo<Vector2Int>() - b.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => (a.ConvertTo<Vector3Int>() - b.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => (a.ConvertTo<Color>() - b.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Negate<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => (-a.ConvertTo<int>()).ConvertTo<T>(),
                float => (-a.ConvertTo<float>()).ConvertTo<T>(),
                double => (-a.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => (-a.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => (-a.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => (-a.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => (-a.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => (-a.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => throw new ArgumentException(),
                _ => throw new ArgumentException(),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Multiply<T>(this T a, float b) where T : IEquatable<T>
        {
            return a switch
            {
                int => (a.ConvertTo<int>() * b).ConvertTo<T>(),
                float => (a.ConvertTo<float>() * b).ConvertTo<T>(),
                double => (a.ConvertTo<double>() * b).ConvertTo<T>(),
                Vector2 => (a.ConvertTo<Vector2>() * b).ConvertTo<T>(),
                Vector3 => (a.ConvertTo<Vector3>() * b).ConvertTo<T>(),
                Vector4 => (a.ConvertTo<Vector4>() * b).ConvertTo<T>(),
                Vector2Int => MathFunc.Multiply(a.ConvertTo<Vector2Int>(), b).ConvertTo<T>(),
                Vector3Int => MathFunc.Multiply(a.ConvertTo<Vector3Int>(), b).ConvertTo<T>(),
                Color => (a.ConvertTo<Color>() * b.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Multiply<T>(this T a, T b) where T : IEquatable<T>
        {
            return a switch
            {
                int => (a.ConvertTo<int>() * b.ConvertTo<int>()).ConvertTo<T>(),
                float => (a.ConvertTo<float>() * b.ConvertTo<float>()).ConvertTo<T>(),
                double => (a.ConvertTo<double>() * b.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Multiply(a.ConvertTo<Vector2>(), b.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Multiply(a.ConvertTo<Vector3>(), b.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Multiply(a.ConvertTo<Vector4>(), b.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.Multiply(a.ConvertTo<Vector2Int>(), b.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.Multiply(a.ConvertTo<Vector3Int>(), b.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => MathFunc.Multiply(a.ConvertTo<Color>(), b.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Divide<T>(this T dividend, T divisor) where T : IEquatable<T>
        {
            return dividend switch
            {
                int => MathFunc.Divide(dividend.ConvertTo<int>(), divisor.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.Divide(dividend.ConvertTo<float>(), divisor.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Divide(dividend.ConvertTo<double>(), divisor.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Divide(dividend.ConvertTo<Vector2>(), divisor.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Divide(dividend.ConvertTo<Vector3>(), divisor.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Divide(dividend.ConvertTo<Vector4>(), divisor.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.Divide(dividend.ConvertTo<Vector2Int>(), divisor.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.Divide(dividend.ConvertTo<Vector3Int>(), divisor.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => MathFunc.Divide(dividend.ConvertTo<Color>(), divisor.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Pow<T>(this T toPow, float power) where T : IEquatable<T>
        {
            return toPow switch
            {
                int => MathFunc.Pow(toPow.ConvertTo<int>(), power).ConvertTo<T>(),
                float => MathFunc.Pow(toPow.ConvertTo<float>(), power).ConvertTo<T>(),
                double => MathFunc.Pow(toPow.ConvertTo<double>(), power).ConvertTo<T>(),
                Vector2 => MathFunc.Pow(toPow.ConvertTo<Vector2>(), power).ConvertTo<T>(),
                Vector3 => MathFunc.Pow(toPow.ConvertTo<Vector3>(), power).ConvertTo<T>(),
                Vector4 => MathFunc.Pow(toPow.ConvertTo<Vector4>(), power).ConvertTo<T>(),
                Vector2Int => MathFunc.Pow(toPow.ConvertTo<Vector2Int>(), power).ConvertTo<T>(),
                Vector3Int => MathFunc.Pow(toPow.ConvertTo<Vector3Int>(), power).ConvertTo<T>(),
                Color => MathFunc.Pow(toPow.ConvertTo<Color>(), power).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        #endregion

        #region Min & Max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>(this T a, T min) where T : struct, IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.Min(a.ConvertTo<int>(), min.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.Min(a.ConvertTo<float>(), min.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Min(a.ConvertTo<double>(), min.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Min(a.ConvertTo<Vector2>(), min.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Min(a.ConvertTo<Vector3>(), min.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Min(a.ConvertTo<Vector4>(), min.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.Min(a.ConvertTo<Vector2Int>(), min.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.Min(a.ConvertTo<Vector3Int>(), min.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => MathFunc.Min(a.ConvertTo<Color>(), min.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>(this IEnumerable<T> enumerable) where T : struct, IEquatable<T>
        {
            return enumerable.Aggregate(Min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Min<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector) 
            where TResult : struct, IEquatable<TResult>
        {
            return enumerable.Select(selector).Aggregate(Min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MinOrDefault<T>(this IEnumerable<T> enumerable) where T : struct, IEquatable<T>
        {
            return MinOrDefault(enumerable, item => item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult MinOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
            where TResult : struct, IEquatable<TResult>
        {
            var list = enumerable.ToList();
            return list.Count == 0 ? default : list.Select(selector).Aggregate(Min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>(this T a, T min) where T : struct, IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.Max(a.ConvertTo<int>(), min.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.Max(a.ConvertTo<float>(), min.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Max(a.ConvertTo<double>(), min.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Max(a.ConvertTo<Vector2>(), min.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Max(a.ConvertTo<Vector3>(), min.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Max(a.ConvertTo<Vector4>(), min.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.Max(a.ConvertTo<Vector2Int>(), min.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.Max(a.ConvertTo<Vector3Int>(), min.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => MathFunc.Max(a.ConvertTo<Color>(), min.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>(this IEnumerable<T> enumerable) where T : struct, IEquatable<T>
        {
            return enumerable.Aggregate(Max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Max<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
            where TResult : struct, IEquatable<TResult>
        {
            return enumerable.Select(selector).Aggregate(Max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MaxOrDefault<T>(this IEnumerable<T> enumerable) where T : struct, IEquatable<T>
        {
            return MaxOrDefault(enumerable, item => item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult MaxOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
            where TResult : struct, IEquatable<TResult>
        {
            var list = enumerable.ToList();
            return list.Count == 0 ? default : list.Select(selector).Aggregate(Max);
        }

        #endregion

        #region Abs & Modulus

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Abs<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.Abs(a.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.Abs(a.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Abs(a.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Abs(a.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Abs(a.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Abs(a.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.Abs(a.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.Abs(a.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => MathFunc.Abs(a.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Modulus<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => a.ConvertTo<int>().Abs(),
                float => a.ConvertTo<float>().Abs(),
                double => a.ConvertTo<double>().Abs(),
                Vector2 => MathFunc.Modulus(a.ConvertTo<Vector2>()),
                Vector3 => MathFunc.Modulus(a.ConvertTo<Vector3>()),
                Vector4 => MathFunc.Modulus(a.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.Modulus(a.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.Modulus(a.ConvertTo<Vector3Int>()),
                Color => MathFunc.Modulus(a.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        #endregion

        #region Sum & Products

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Products<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => a.ConvertTo<double>(),
                float => a.ConvertTo<double>(),
                double => a.ConvertTo<double>(),
                Vector2 => MathFunc.Products(a.ConvertTo<Vector2>()),
                Vector3 => MathFunc.Products(a.ConvertTo<Vector3>()),
                Vector4 => MathFunc.Products(a.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.Products(a.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.Products(a.ConvertTo<Vector3Int>()),
                Color => MathFunc.Products(a.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sum<T>(this T a) where T : IEquatable<T>
        {
            return a switch
            {
                int => a.ConvertTo<double>(),
                float => a.ConvertTo<double>(),
                double => a.ConvertTo<double>(),
                Vector2 => MathFunc.Sum(a.ConvertTo<Vector2>()),
                Vector3 => MathFunc.Sum(a.ConvertTo<Vector3>()),
                Vector4 => MathFunc.Sum(a.ConvertTo<Vector4>()),
                Vector2Int => MathFunc.Sum(a.ConvertTo<Vector2Int>()),
                Vector3Int => MathFunc.Sum(a.ConvertTo<Vector3Int>()),
                Color => MathFunc.Sum(a.ConvertTo<Color>()),
                _ => throw new ArgumentException()
            };
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance<T>(this T a, T b) where T : IEquatable<T>
        {
            return a.Subtract(b).Modulus();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Lerp<T>(this T from, T to, float t) where T : IEquatable<T>
        {
            return from switch
            {
                int => MathFunc.Lerp(from.ConvertTo<int>(), to.ConvertTo<int>(), t).ConvertTo<T>(),
                float => MathFunc.Lerp(from.ConvertTo<float>(), to.ConvertTo<float>(), t).ConvertTo<T>(),
                double => MathFunc.Lerp(from.ConvertTo<double>(), to.ConvertTo<double>(), t).ConvertTo<T>(),
                Vector2 => MathFunc.Lerp(from.ConvertTo<Vector2>(), to.ConvertTo<Vector2>(), t).ConvertTo<T>(),
                Vector3 => MathFunc.Lerp(from.ConvertTo<Vector3>(), to.ConvertTo<Vector3>(), t).ConvertTo<T>(),
                Vector4 => MathFunc.Lerp(from.ConvertTo<Vector4>(), to.ConvertTo<Vector4>(), t).ConvertTo<T>(),
                Vector2Int => MathFunc.Lerp(from.ConvertTo<Vector2Int>(), to.ConvertTo<Vector2Int>(), t).ConvertTo<T>(),
                Vector3Int => MathFunc.Lerp(from.ConvertTo<Vector3Int>(), to.ConvertTo<Vector3Int>(), t).ConvertTo<T>(),
                Color => MathFunc.Lerp(from.ConvertTo<Color>(), to.ConvertTo<Color>(), t).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Lerp<T>(this T from, T to, float t, float power) where T : IEquatable<T>
        {
            return from switch
            {
                int => MathFunc.Lerp(from.ConvertTo<int>(), to.ConvertTo<int>(), t, power).ConvertTo<T>(),
                float => MathFunc.Lerp(from.ConvertTo<float>(), to.ConvertTo<float>(), t, power).ConvertTo<T>(),
                double => MathFunc.Lerp(from.ConvertTo<double>(), to.ConvertTo<double>(), t, power).ConvertTo<T>(),
                Vector2 => MathFunc.Lerp(from.ConvertTo<Vector2>(), to.ConvertTo<Vector2>(), t, power).ConvertTo<T>(),
                Vector3 => MathFunc.Lerp(from.ConvertTo<Vector3>(), to.ConvertTo<Vector3>(), t, power).ConvertTo<T>(),
                Vector4 => MathFunc.Lerp(from.ConvertTo<Vector4>(), to.ConvertTo<Vector4>(), t, power).ConvertTo<T>(),
                Vector2Int => MathFunc.Lerp(from.ConvertTo<Vector2Int>(), to.ConvertTo<Vector2Int>(), t, power).ConvertTo<T>(),
                Vector3Int => MathFunc.Lerp(from.ConvertTo<Vector3Int>(), to.ConvertTo<Vector3Int>(), t, power).ConvertTo<T>(),
                Color => MathFunc.Lerp(from.ConvertTo<Color>(), to.ConvertTo<Color>(), t, power).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Normalize<T>(this T a, T min, T max) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.Normalize(a.ConvertTo<int>(), min.ConvertTo<int>(), max.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.Normalize(a.ConvertTo<float>(), min.ConvertTo<float>(), max.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.Normalize(a.ConvertTo<double>(), min.ConvertTo<double>(), max.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.Normalize(a.ConvertTo<Vector2>(), min.ConvertTo<Vector2>(), max.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.Normalize(a.ConvertTo<Vector3>(), min.ConvertTo<Vector3>(), max.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.Normalize(a.ConvertTo<Vector4>(), min.ConvertTo<Vector4>(), max.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.Normalize(a.ConvertTo<Vector2Int>(), min.ConvertTo<Vector2Int>(), max.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.Normalize(a.ConvertTo<Vector3Int>(), min.ConvertTo<Vector3Int>(), max.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => throw new ArgumentException(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PointSymmetric<T>(this T a, T point) where T : IEquatable<T>
        {
            return a switch
            {
                int => MathFunc.PointSymmetric(a.ConvertTo<int>(), point.ConvertTo<int>()).ConvertTo<T>(),
                float => MathFunc.PointSymmetric(a.ConvertTo<float>(), point.ConvertTo<float>()).ConvertTo<T>(),
                double => MathFunc.PointSymmetric(a.ConvertTo<double>(), point.ConvertTo<double>()).ConvertTo<T>(),
                Vector2 => MathFunc.PointSymmetric(a.ConvertTo<Vector2>(), point.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => MathFunc.PointSymmetric(a.ConvertTo<Vector3>(), point.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => MathFunc.PointSymmetric(a.ConvertTo<Vector4>(), point.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => MathFunc.PointSymmetric(a.ConvertTo<Vector2Int>(), point.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => MathFunc.PointSymmetric(a.ConvertTo<Vector3Int>(), point.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => throw new ArgumentException(),
                _ => throw new ArgumentException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomRange<T>(this T a, T min) where T : IEquatable<T>
        {
            return a switch
            {
                int => RandomFunc.RandomRange(a.ConvertTo<int>(), min.ConvertTo<int>()).ConvertTo<T>(),
                float => RandomFunc.RandomRange(a.ConvertTo<float>(), min.ConvertTo<float>()).ConvertTo<T>(),
                double => throw new ArgumentException(),
                Vector2 => RandomFunc.RandomRange(a.ConvertTo<Vector2>(), min.ConvertTo<Vector2>()).ConvertTo<T>(),
                Vector3 => RandomFunc.RandomRange(a.ConvertTo<Vector3>(), min.ConvertTo<Vector3>()).ConvertTo<T>(),
                Vector4 => RandomFunc.RandomRange(a.ConvertTo<Vector4>(), min.ConvertTo<Vector4>()).ConvertTo<T>(),
                Vector2Int => RandomFunc.RandomRange(a.ConvertTo<Vector2Int>(), min.ConvertTo<Vector2Int>()).ConvertTo<T>(),
                Vector3Int => RandomFunc.RandomRange(a.ConvertTo<Vector3Int>(), min.ConvertTo<Vector3Int>()).ConvertTo<T>(),
                Color => RandomFunc.RandomRange(a.ConvertTo<Color>(), min.ConvertTo<Color>()).ConvertTo<T>(),
                _ => throw new ArgumentException()
            };
        }
    }
}

