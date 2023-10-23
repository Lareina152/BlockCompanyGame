using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Basis;
using UnityEngine;

namespace Basis
{
    public static class Assert
    {
        private static void AssertLog(this Note note, bool condition, string log)
        {
            if (condition == false)
            {
                note.Error(log);
            }
        }

        #region General

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIs<T>(this Note note, T value, T target, string valueName)
        {
            note.AssertLog(value.Equals(target), $"{valueName} is not {target}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsNot<T>(this Note note, T value, T target, string valueName)
        {
            note.AssertLog(value.Equals(target) == false, $"{valueName} is {target}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsNull<T>(this Note note, T value, string valueName)
        {
            note.AssertLog(value == null, $"{valueName} is Not Null");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsNotNull<T>(this Note note, T value, string valueName)
        {
            note.AssertLog(value != null, $"{valueName} is Null");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertAreEqual<T>(this Note note, T value, T expected, string valueName)
        {
            note.AssertLog(value.Equals(expected), $"{valueName} is Not Equal to Expected Value");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertAreNotEqual<T>(this Note note, T actual, T expected, string valueName)
        {
            note.AssertLog(actual.Equals(expected) == false, $"{valueName} is Equal to Expected Value");
        }

        #endregion

        #region Bool

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsTrue(this Note note, bool value, string valueName)
        {
            note.AssertLog(value == true, $"{valueName} is False");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsFalse(this Note note, bool value, string valueName)
        {
            note.AssertLog(value == false, $"{valueName} is True");
        }

        #endregion

        #region Num & Comparable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelow<T>(this Note note, T value, T comparison, string valueName, string comparisonName = "") 
            where T : IComparable
        {
            if (comparisonName.IsNullOrEmpty())
            {
                comparisonName = value.ToString();
            }
            note.AssertLog(value.Below(comparison), $"{valueName} is Not Below the {comparisonName}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAbove<T>(this Note note, T value, T comparison, string valueName, string comparisonName = "") 
            where T : IComparable
        {
            if (comparisonName.IsNullOrEmpty())
            {
                comparisonName = value.ToString();
            }
            note.AssertLog(value.Above(comparison), $"{valueName} is Not Above the {comparisonName}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelowOrEqual<T>(this Note note, T value, T comparison, string valueName, string comparisonName = "") 
            where T : IComparable
        {
            if (comparisonName.IsNullOrEmpty())
            {
                comparisonName = value.ToString();
            }
            note.AssertLog(value.BelowOrEqual(comparison), $"{valueName} is Not Below Or Equal the {comparisonName}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAboveOrEqual<T>(this Note note, T value, T comparison, string valueName, string comparisonName = "") 
            where T : IComparable
        {
            if (comparisonName.IsNullOrEmpty())
            {
                comparisonName = value.ToString();
            }
            note.AssertLog(value.AboveOrEqual(comparison), $"{valueName} is Not Above Or Equal the {comparisonName}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelow(this Note note, Vector2 value, Vector2 comparison)
        {
            if (value.AllNumberBelow(comparison) == false)
            {
                note.Error("Value is Not Below the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAbove(this Note note, Vector2 value, Vector2 comparison)
        {
            if (value.AllNumberAbove(comparison) == false)
            {
                note.Error("Value is Not Above the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelowOrEqual(this Note note, Vector2 value, Vector2 comparison)
        {
            if (value.AllNumberBelowOrEqual(comparison) == false)
            {
                note.Error("Value is Not Below Or Equal the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAboveOrEqual(this Note note, Vector2 value, Vector2 comparison)
        {
            if (value.AllNumberAboveOrEqual(comparison) == false)
            {
                note.Error("Value is Not Above Or Equal the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelow(this Note note, Vector3 value, Vector3 comparison)
        {
            if (value.AllNumberBelow(comparison) == false)
            {
                note.Error("Value is Not Below the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAbove(this Note note, Vector3 value, Vector3 comparison)
        {
            if (value.AllNumberAbove(comparison) == false)
            {
                note.Error("Value is Not Above the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelowOrEqual(this Note note, Vector3 value, Vector3 comparison)
        {
            if (value.AllNumberBelowOrEqual(comparison) == false)
            {
                note.Error("Value is Not Below Or Equal the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAboveOrEqual(this Note note, Vector3 value, Vector3 comparison)
        {
            if (value.AllNumberAboveOrEqual(comparison) == false)
            {
                note.Error("Value is Not Above Or Equal the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelow(this Note note, Vector2Int value, Vector2Int comparison)
        {
            if (value.AllNumberBelow(comparison) == false)
            {
                note.Error("Value is Not Below the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAbove(this Note note, Vector2Int value, Vector2Int comparison)
        {
            if (value.AllNumberAbove(comparison) == false)
            {
                note.Error("Value is Not Above the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelowOrEqual(this Note note, Vector2Int value, Vector2Int comparison)
        {
            if (value.AllNumberBelowOrEqual(comparison) == false)
            {
                note.Error("Value is Not Below Or Equal the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAboveOrEqual(this Note note, Vector2Int value, Vector2Int comparison)
        {
            if (value.AllNumberAboveOrEqual(comparison) == false)
            {
                note.Error("Value is Not Above Or Equal the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelow(this Note note, Vector3Int value, Vector3Int comparison)
        {
            if (value.AllNumberBelow(comparison) == false)
            {
                note.Error("Value is Not Below the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAbove(this Note note, Vector3Int value, Vector3Int comparison)
        {
            if (value.AllNumberAbove(comparison) == false)
            {
                note.Error("Value is Not Above the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsBelowOrEqual(this Note note, Vector3Int value, Vector3Int comparison)
        {
            if (value.AllNumberBelowOrEqual(comparison) == false)
            {
                note.Error("Value is Not Below Or Equal the Comparison");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAboveOrEqual(this Note note, Vector3Int value, Vector3Int comparison)
        {
            if (value.AllNumberAboveOrEqual(comparison) == false)
            {
                note.Error("Value is Not Above Or Equal the Comparison");
            }
        }

        #endregion

        #region Type

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsClass(this Note note, Type type)
        {
            if (type.IsClass == false)
            {
                note.Error($"Type:{type}不是Class");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsNotClass(this Note note, Type type)
        {
            if (type.IsClass)
            {
                note.Error($"Type:{type}是Class");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsDerivedFrom(this Note note, Type type, Type parentType, bool includingSelf, bool includingInterfaces,
            bool includingGeneric)
        {
            if (type.IsDerivedFrom(parentType, includingSelf, includingInterfaces, includingGeneric) == false)
            {
                note.Error($"{type}不是{parentType}的子类");
            }
        }

        #endregion

        #region String

        public static void AssertIsNullOrEmpty(this Note note, string value, string valueName)
        {
            note.AssertLog(string.IsNullOrEmpty(value), $"{valueName}不是Null或空字符串");
        }

        public static void AssertIsNotNullOrEmpty(this Note note, string value, string valueName)
        {
            note.AssertLog(string.IsNullOrEmpty(value) == false, $"{valueName}不是Null或空字符串");
        }

        public static void AssertIsNullOrEmptyAfterTrim(this Note note, string value, string valueName)   
        {
            note.AssertLog(value.IsNullOrEmptyAfterTrim(), $"{valueName}不是Null或剪裁后的空字符串");
        }

        public static void AssertIsNotNullOrEmptyAfterTrim(this Note note, string value, string valueName)
        {
            note.AssertLog(value.IsNullOrEmptyAfterTrim() == false, $"{valueName}是Null或剪裁后的空字符串");
        }

        #endregion
    }
}

