using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System;

namespace Basis{
    // float utility and extention functions
    public static class FloatUtility{

        private const float EPSILON = float.Epsilon;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatEquals(this float a, float b){
            return Math.Abs(a - b) < EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatGreaterThan(float a, float b){
            return (a - b) > EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatGreaterThanOrEquals(float a, float b){
            return FloatGreaterThan(a, b) || FloatEquals(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatLessThan(float a, float b){
            return (b - a) > EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatLessThanOrEquals(float a, float b){
            return FloatLessThan(a, b) || FloatEquals(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatIsZero(float a){
            return Math.Abs(a) < EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float angle){
            return (float)Math.Cos(angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float angle){
            return (float)Math.Sin(angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float angle){
            return (float)Math.Tan(angle);
        }
    }

    public static class FLoatExtention{
        private const float EPSILON = float.Epsilon;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatEquals(float a, float b){
            return Math.Abs(a - b) < EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatGreaterThan(this float a, float b){
            return (a - b) > EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatGreaterThanOrEquals(this float a, float b){
            return FloatGreaterThan(a, b) || FloatEquals(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatLessThan(this float a, float b){
            return (b - a) > EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatLessThanOrEquals(this float a, float b){
            return FloatLessThan(a, b) || FloatEquals(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatIsZero(this float a){
            return Math.Abs(a) < EPSILON;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FloatCos(this float angle){
            return (float)Math.Cos(angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FloatSin(this float angle){
            return (float)Math.Sin(angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FloatTan(this float angle){
            return (float)Math.Tan(angle);
        }
    }
}
