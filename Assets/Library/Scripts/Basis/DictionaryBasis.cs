using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Basis
{
    public static class DictionaryBasis
    {
        public static void FillMissingKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                if (dict.ContainsKey(key) == false)
                {
                    dict[key] = (TValue)typeof(TValue).CreateInstance();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TKey, bool> selector)
        {
            return dict.Keys.ToArray().Any(selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TKey, bool> selector)
        {
            return dict.Keys.ToArray().All(selector);
        }

        public static void ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey oldKey, TKey newKey)
        {
            if (dict.TryGetValue(oldKey, out var value) == false)
            {
                Note.note.Warning($"键{oldKey}不存在");
                return;
            }

            dict.Remove(oldKey);

            if (dict.ContainsKey(newKey))
            {
                Note.note.Warning($"字典键:{newKey}下的值已被原来{oldKey}下的值取代");
            }

            dict[newKey] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TKey> GetKeysByValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value)
        {
            return dict.Where(kvp => kvp.Value.Equals(value)).
                Select(kvp => kvp.Key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TKey GetFirstKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value)
        {
            return dict.Where(kvp => kvp.Value.Equals(value)).
                Select(kvp => kvp.Key).FirstOrDefault();
        }
    }
}

