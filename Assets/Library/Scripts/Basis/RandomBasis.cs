using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Basis {
    public static class RandomFunc
    {
        #region Choose

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Choose<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToArray().Choose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Choose<T>(this IList<T> list)
        {
            return list.Count == 0 ? default : list[Random.Range(0, list.Count)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Choose<T>(this IList<T> list, int count)
        {
            return list.Get(GenerateUniqueIntegers(count, 0, list.Count - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyValuePair<TKey,TValue> Choose<TKey,TValue>(this Dictionary<TKey,TValue> dict) {
            var resultKey = Choose(dict.Keys.ToList());
            return new KeyValuePair<TKey,TValue>(resultKey, dict[resultKey]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TKey ChooseKey<TKey,TValue>(this Dictionary<TKey,TValue> dict) {
            return Choose(dict.Keys.ToList());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue ChooseValue<TKey,TValue>(this Dictionary<TKey,TValue> dict) {
            return Choose(dict.Values.ToList());
        }

        public static T Choose<T>(T[] obj, float[] prob)
        {
            if (obj.Length == 0)
            {
                return default;
            }
            if (obj.Length == 1)
            {
                return obj[0];
            }

            if (prob.Length == 1)
            {
                return obj[0];
            }

            int length = prob.Length;

            if (prob.Length > obj.Length)
            {
                length = obj.Length;
            }

            float sum = prob.Sum(length);

            if (sum == 0)
            {
                return default;
            }

            float randomProb = Random.Range(0, sum);

            float temp = 0;
            for (int i = 0; i < length; i++)
            {
                temp += prob[i];
                if (temp >= randomProb)
                {
                    return obj[i];
                }
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Choose<T>(List<T> obj, List<float> prob)
        {
            return Choose(obj.ToArray(), prob.ToArray());
        }

        #endregion

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        #region RandomRange

        #region Range

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RandomRange(this int min, int max)
        {
            return Random.Range(min, max + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RandomRange(this int length)
        {
            return RandomRange(0, length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomRange(this float min, float max)
        {
            return Random.Range(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomRange(this float length)
        {
            return RandomRange(0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int RandomRange(this Vector2Int minPos, Vector2Int maxPos)
        {
            return new(minPos.x.RandomRange(maxPos.x), minPos.y.RandomRange(maxPos.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int RandomRange(this Vector2Int size)
        {
            return RandomRange(Vector2Int.zero, size - Vector2Int.one);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int RandomRange(this Vector3Int minPos, Vector3Int maxPos)
        {
            return new(minPos.x.RandomRange(maxPos.x), minPos.y.RandomRange(maxPos.y), minPos.z.RandomRange(maxPos.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int RandomRange(this Vector3Int size)
        {
            return RandomRange(Vector3Int.zero, size - Vector3Int.one);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RandomRange(this Vector2 minPos, Vector2 maxPos)
        {
            return new(minPos.x.RandomRange(maxPos.x), minPos.y.RandomRange(maxPos.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RandomRange(this Vector2 size)
        {
            return RandomRange(Vector2.zero, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RandomRange(this Vector3 minPos, Vector3 maxPos)
        {
            return new(minPos.x.RandomRange(maxPos.x), minPos.y.RandomRange(maxPos.y), minPos.z.RandomRange(maxPos.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RandomRange(this Vector3 size)
        {
            return RandomRange(Vector3.zero, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 RandomRange(this Vector4 minPos, Vector4 maxPos)
        {
            return new(minPos.x.RandomRange(maxPos.x), minPos.y.RandomRange(maxPos.y), minPos.z.RandomRange(maxPos.z),
                minPos.w.RandomRange(maxPos.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 RandomRange(this Vector4 size)
        {
            return RandomRange(Vector4.zero, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color RandomRange(this Color minPos, Color maxPos)
        {
            return new(minPos.r.RandomRange(maxPos.r), minPos.g.RandomRange(maxPos.g), minPos.b.RandomRange(maxPos.b),
                minPos.a.RandomRange(maxPos.a));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color RandomRange(this Color size)
        {
            return RandomRange(new(0, 0, 0, 0), size);
        }

        #endregion

        public static IEnumerable<int> SeveralRandomRange(this int count, int min, int max)
        {
            return count.DoFuncNTimes(() => RandomRange(min, max));
        }

        public static IEnumerable<int> SeveralRandomRange(this int count, int length)
        {
            return SeveralRandomRange(count, 0, length - 1);
        }

        public static IEnumerable<int> GenerateUniqueIntegers(this int count, int min, int max)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<int>();
            }

            int range = max - min + 1;

            if (range <= 0)
            {
                return Enumerable.Empty<int>();
            }

            count = count.ClampMax(range);

            if (count < range / 3)
            {
                var numbers = new HashSet<int>();

                while (numbers.Count < count)
                {
                    int number = RandomRange(min, max);
                    numbers.Add(number);
                }

                return numbers;
            }
            else
            {
                var container = min.GetSteppedPoints(max).ToList();

                container.Shuffle();

                return container.GetRange(0, count);
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> GenerateUniqueIntegers(this int count, int length)
        {
            return GenerateUniqueIntegers(count, 0, length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector2Int> GenerateUniqueVector2Ints(this int count, Vector2Int minPos, Vector2Int maxPos)
        {
            Vector2Int range = maxPos - minPos + Vector2Int.one;

            int totalLength = range.Products();

            foreach (var index in GenerateUniqueIntegers(count, totalLength))
            {
                int x = index / range.y;
                int y = index - x * range.y;
                yield return new(x + minPos.x, y + minPos.y);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector2Int> GenerateUniqueVector2Ints(this int count, Vector2Int size)
        {
            return GenerateUniqueVector2Ints(count, Vector2Int.zero, size - Vector2Int.one);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GenerateUniqueVector3Ints(this int count, Vector3Int minPos, Vector3Int maxPos)
        {
            Vector3Int range = maxPos - minPos + Vector3Int.one;

            int totalLength = range.Products();

            foreach (var index in GenerateUniqueIntegers(count, totalLength))
            {
                int rangeYZ = range.y * range.z;
                int x = index / rangeYZ;
                int rangeDivideByYZ = index - x * rangeYZ;
                int y = rangeDivideByYZ / range.z;
                int z = rangeDivideByYZ - y * range.z;
                yield return new(x + minPos.x, y + minPos.y, z + minPos.z);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector3Int> GenerateUniqueVector3Ints(this int count, Vector3Int size)
        {
            return GenerateUniqueVector3Ints(count, Vector3Int.zero, size - Vector3Int.one);
        }

        #endregion

        #region RandomKCubeShape

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeInteger RandomRangeInteger(this int from, int to)
        {
            if (to < from)
            {
                Note.note.Warning("to < from");
                return new();
            }
            int start = RandomRange(from, to);
            int length = RandomRange(0, to - from + 1);

            int end = (start + length).Repeat(from, to);

            return start > end ? new(end, start) : new(start, end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<RangeInteger> SeveralRandomRangeInteger(this int count, int from, int to)
        {
            return count.DoFuncNTimes(() => RandomRangeInteger(from, to));
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RandomTrue01(this float ratio)
        {
            return Random.value <= ratio;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RandomTruePercents(this float ratio)
        {
            return Random.value * 100 <= ratio;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RandomTruePercents(this int ratio)
        {
            return RandomTruePercents((float)ratio);
        }
    }
}

