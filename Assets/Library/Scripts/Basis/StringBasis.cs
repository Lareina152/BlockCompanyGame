using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Basis {

    public static class StringFunc {
        public static int GetLineCount(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return 1 + text.Count(c => c == '\n');
        }

        #region Split & Join

        public static List<string> Split(string oldText, int length) {
            var result = new List<string>();

            int totalLength = oldText.Length;
            int amount = totalLength / length;
            
            for (int i = 0; i < amount; i++) {
                result.Add(oldText.Substring(i * length, length));
            }

            if (amount * length < totalLength) {
                result.Add(oldText[(amount * length)..]);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Join<T>(this string separator, IEnumerable<T> enumerable)
        {
            return string.Join(separator, enumerable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Join<T>(this IEnumerable<T> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }

        #endregion

        #region Check

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmptyAfterTrim(this string str)
        {
            return str == null || IsEmptyAfterTrim(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmptyAfterTrim([NotNull] this string str)
        {
            return str.Trim() == "";
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this float value, int decimalPlaces)
        {
            return value.ToString("F" + decimalPlaces.ClampMin(0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this Vector2 value, int decimalPlaces)
        {
            return $"({value.x.ToString(decimalPlaces)},{value.y.ToString(decimalPlaces)})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this Vector3 value, int decimalPlaces)
        {
            return $"({value.x.ToString(decimalPlaces)},{value.y.ToString(decimalPlaces)},{value.z.ToString(decimalPlaces)})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this Vector4 value, int decimalPlaces)
        {
            return $"({value.x.ToString(decimalPlaces)},{value.y.ToString(decimalPlaces)},{value.z.ToString(decimalPlaces)}" +
                   $"{value.w.ToString(decimalPlaces)})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString<T>(this IEnumerable<T> enumerable, string step = ",")
        {
            return enumerable == null ? "Null" : step.Join(enumerable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this float num)
        {
            return (num * 255).Round().ToString("X2");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this int num)
        {
            return num.ToString("X2");
        }

        public static string CapitalizeFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            
            return char.ToUpper(str[0]) + str[1..];
        }

        #region Words

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> GetWords(this string input)
        {
            var words = new List<string>();
            StringBuilder currentWord = new StringBuilder();
            var isDigit = true;
            var isCurrentWorldAllUpper = true;

            void ClearCurrentWorld()
            {
                if (currentWord.Length > 0)
                {
                    words.Add(currentWord.ToString());
                    currentWord.Clear();
                    isCurrentWorldAllUpper = true;
                    isDigit = true;
                }
            }

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsLetter(c))
                {
                    if (currentWord.Length > 1 && isDigit)
                    {
                        ClearCurrentWorld();
                    }

                    isDigit = false;

                    if (char.IsUpper(c))
                    {
                        if (currentWord.Length > 0 && isCurrentWorldAllUpper)
                        {
                            if (i != input.Length - 1)
                            {
                                var nextChar = input[i + 1];

                                if (char.IsLower(nextChar))
                                {
                                    ClearCurrentWorld();
                                }
                            }
                        }
                        else
                        {
                            ClearCurrentWorld();
                        }
                    }
                    else
                    {
                        isCurrentWorldAllUpper = false;
                    }
                }
                else if (char.IsDigit(c))
                {
                    if (currentWord.Length == 0 || isDigit == false)
                    {
                        ClearCurrentWorld();
                    }
                }
                else
                {
                    ClearCurrentWorld();
                    continue;
                }
                
                currentWord.Append(c);
            }

            if (currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
            }

            return words;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToSnakeCase(this string input)
        {
            return input.GetWords().Select(word => word.ToLower()).ToString("_");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToPascalCase(this string input, string step = "")
        {
            return input.GetWords().Select(CapitalizeFirstLetter).ToString(step);
        }

        #endregion
    }
}

