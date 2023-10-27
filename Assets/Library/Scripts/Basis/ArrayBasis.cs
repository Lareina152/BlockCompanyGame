using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;

namespace Basis
{
    public static class ArrayFunc
    {
        #region Create && Init

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[,,] CreateArray<T>(this Vector3Int size)
        {
            if (size.AnyNumberBelowOrEqual(0))
            {
                Note.note.Warning($"CreateArray传入的size参数有分量小于等于0");
                return null;
            }
            return new T[size.x, size.y, size.z];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[,] CreateArray<T>(this Vector2Int size)
        {
            if (size.AnyNumberBelowOrEqual(0))
            {
                Note.note.Warning($"CreateArray传入的size参数有分量小于等于0");
                return null;
            }
            return new T[size.x, size.y];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitArray<T>(this T[,] array) where T : new()
        {
            foreach (var pos in array.GetSize().GetRectangleOfPoints())
            {
                array.Set(pos, new());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitArray<T>(this T[,,] array) where T : new()
        {
            foreach (var pos in array.GetSize().GetCubeOfPoints())
            {
                array.Set(pos, new());
            }
        }

        #endregion

        #region Get && Set && Remove

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Get<T>(this IList<T> list, RangeInteger range)
        {
            return range.GetAllPoints().Select(index => list[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Get<T>(this IList<T> list, IEnumerable<int> indices)
        {
            return indices.Select(index => list[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Get<T>(this IList<T> list, params int[] indices)
        {
            return list.Get(indices.AsEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrBound<T>(this IList<T> list, int index)
        {
            return list[index.Clamp(0, list.Count - 1)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> GetOrBound<T>(this IList<T> list, IEnumerable<int> indices)
        {
            return indices.Select(list.GetOrBound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> GetOrBound<T>(this IList<T> list, params int[] indices)
        {
            return list.GetOrBound(indices.AsEnumerable());
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static IEnumerable<float> GetFirstFourAsVector(this IEnumerable<float> enumerable)
        //{

        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Replace<T>(this T[,,] array, Vector3Int pos, T content)
        {
            var oldValue = array[pos.x, pos.y, pos.z];
            array[pos.x, pos.y, pos.z] = content;
            return oldValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this T[,,] array, Vector3Int pos, T content)
        {
            array[pos.x, pos.y, pos.z] = content;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this T[,,] array, T content)
        {
            foreach (var pos in array.GetSize().GetCubeOfPoints())
            {
                array.Set(pos, content);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[,,] array, Vector3Int pos)
        {
            return array[pos.x, pos.y, pos.z];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Replace<T>(this T[,] array, Vector2Int pos, T content)
        {
            var oldValue = array[pos.x, pos.y];
            array[pos.x, pos.y] = content;
            return oldValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this T[,] array, Vector2Int pos, T content)
        {
            array[pos.x, pos.y] = content;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this T[,] array, T content)
        {
            foreach (var pos in array.GetSize().GetRectangleOfPoints())
            {
                array.Set(pos, content);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Pop<T>(this T[,] array, Vector2Int pos)
        {
            var popItem = array[pos.x, pos.y];
            array[pos.x, pos.y] = default;
            return popItem;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[,] array, Vector2Int pos)
        {
            return array[pos.x, pos.y];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> GetByX<T>(this T[,] array, int x)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                yield return array[x, y];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> GetByXRange<T>(this T[,] array, int xFrom, int xTo)
        {
            for (int x = xFrom; x <= xTo; x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    yield return array[x, y];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> GetByY<T>(this T[,] array, int y)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                yield return array[x, y];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int GetSize<T>(this T[,,] array)
        {
            if (array == null)
            {
                return default;
            }

            return new(array.GetLength(0), array.GetLength(1), array.GetLength(2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int GetSize<T>(this T[,] array)
        {
            if (array == null)
            {
                return default;
            }

            return new(array.GetLength(0), array.GetLength(1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAllNull<T>(this IList<T> list) where T : class
        {
            while (list.Remove(null)) {}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Remove<T>(this IList<T> list, Func<T, bool> condition)
        {
            bool hasRemoved = false;

            list.Examine(item =>
            {
                if (condition(item))
                {
                    list.Remove(item);
                    hasRemoved = true;
                }
            });

            return hasRemoved;
        }

        #endregion

        #region LinqExtensions

        #region Contains

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAll<T>(this IEnumerable<T> enumerable, IEnumerable<T> items)
        {
            return items.All(enumerable.Contains);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAny<T>(this IEnumerable<T> enumerable, IEnumerable<T> items)
        {
            return items.Any(enumerable.Contains);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsSame<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ContainsSame(item => item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsSame<T, TSelector>(this IEnumerable<T> enumerable, Func<T, TSelector> selector)
        {
            var array = enumerable.ToArray();

            if (array.Length <= 1)
            {
                return false;
            }

            return array.Distinct(selector).Count() < array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsNull<T>(this IEnumerable<T> enumerable) where T : class
        {
            if (enumerable == null)
            {
                return false;
            }

            return enumerable.Any(item => item == null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsNull<T, TSelector>(this IEnumerable<T> enumerable, Func<T, TSelector> selector) where T : class
        {
            if (enumerable == null)
            {
                return false;
            }

            return enumerable.Select(selector).Any(item => item == null);
        }

        #endregion

        #region Bool

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this IEnumerable<bool> enumerable)
        {
            return enumerable.Any(item => item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<T>(this IEnumerable<T> enumerable, Func<int, T, bool> predicate)
        {
            foreach (var (i, item) in enumerable.Enumerate())
            {
                if (predicate(i, item))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this IEnumerable<bool> enumerable)
        {
            return enumerable.All(item => item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All<T>(this IEnumerable<T> enumerable, Func<int, T, bool> predicate)
        {
            foreach (var (i, item) in enumerable.Enumerate())
            {
                if (predicate(i, item) == false)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Distinct && Duplicates

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveSame<T>(this IList<T> list)
        {
            list.RemoveSame(item => item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveSame<T, TSelector>(this IList<T> list, Func<T, TSelector> selector)
        {
            if (list.Count == 0)
            {
                return;
            }

            List<TSelector> items = new();

            foreach (var parent in list.ToArray())
            {
                var newItem = selector(parent);

                if (items.Contains(newItem))
                {
                    list.Remove(parent);
                }

                items.Add(newItem);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> Distinct<TSource, TSelector>(this IEnumerable<TSource> enumerable, Func<TSource, TSelector> selector)
        {
            var seenKeys = new HashSet<TSelector>();
            foreach (TSource element in enumerable)
            {
                if (seenKeys.Add(selector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IList<T> MergeDuplicates<T, TSelector>(this IList<T> list, Func<T, TSelector> selector,
            Func<T, T, T> merge)
        {
            var afterMerge = new List<T>();

            var completeIndices = new NativeList<int>(Allocator.Temp);

            for (int thisIndex = 0; thisIndex < list.Count; thisIndex++)
            {
                if (completeIndices.Contains(thisIndex))
                {
                    continue;
                }

                var sameItems = new List<T>();

                for (int otherIndex = thisIndex + 1; otherIndex < list.Count; otherIndex++)
                {
                    if (selector(list[thisIndex]).ClassOrStructEquals(selector(list[otherIndex])))
                    {
                        sameItems.Add(list[otherIndex]);
                        completeIndices.Add(otherIndex);
                    }
                }

                if (sameItems.Count > 0)
                {
                    sameItems.Add(list[thisIndex]);

                    afterMerge.Add(sameItems.Aggregate(merge));
                }
                else
                {
                    afterMerge.Add(list[thisIndex]);
                }
            }

            return afterMerge;
        }

        #endregion

        #region Examine

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> DoFuncNTimes<T>(this int n, Func<T> func)
        {
            for (int i = 0; i < n; i++)
            {
                yield return func();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DoActionNTimes(this int n, Action func)
        {
            for (int i = 0; i < n; i++)
            {
                func();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Examine<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable.ToArray())
            {
                action(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Examine<T>(this IList<T> list, Func<T, T> func)
            where T : struct
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = func(list[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExamineKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TKey, TKey> func)
            where TKey : struct
        {
            foreach (var key in dict.Keys.ToArray())
            {
                var newKey = func(key);

                var oldValue = dict[key];
                dict.Remove(key);
                dict[newKey] = oldValue;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<bool> ExamineIf<T>(this IEnumerable<T> enumerable, Func<T, bool> condition, Action<T> action)
        {
            var results = new List<bool>();
            enumerable.Examine(item =>
            {
                var result = condition(item);
                if (result) action(item);
                results.Add(result);
            });

            return results;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(Vector3Int, T)> Enumerate<T>(this T[,,] array, Vector3Int indexOffset = default)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                int x = indexOffset.x + i;
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    int y = indexOffset.y + j;
                    for (int k = 0; k < array.GetLength(2); k++)
                    {
                        int z = indexOffset.z + k;
                        yield return (new(x, y, z), array[i, j, k]);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(Vector3Int, T)> Enumerate<T>(this T[,] array, Vector3Int indexOffset = default)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                int x = indexOffset.x + i;
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    int y = indexOffset.y + j;
                    yield return (new(x, y), array[i, j]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(int index, T)> Enumerate<T>(this IEnumerable<T> array, int indexOffset = 0)
        {
            int index = 0;
            foreach (var item in array)
            {
                yield return (index + indexOffset, item);
                index++;
            }
        }

        #endregion

        #region First && Last

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FirstItems<T>(this IEnumerable<T> enumerable, int totalCount)
        {
            int count = 0;
            foreach (T item in enumerable)
            {
                if (count < totalCount)
                {
                    yield return item;
                    count++;
                }
                else
                {
                    yield break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, int startIndex) where T : class
        {
            return enumerable.FirstOrDefault(item => item != null, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> selector, int startIndex)
        {
            return enumerable.FirstOrDefault((i, item) => i >= startIndex && selector(item));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, Func<int, T, bool> selector,
            int indexOffset = 0)
        {
            foreach (var (i, item) in enumerable.Enumerate(indexOffset))
            {
                if (selector(i, item))
                {
                    return item;
                }
            }

            return default;
        }

        #endregion

        #region Select

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> enumerable,
            Func<int, T, TResult> selector, int indexOffset = 0)
        {
            foreach (var (i, item) in enumerable.Enumerate(indexOffset))
            {
                yield return selector(i, item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> enumerable)
            where TResult : T
        {
            foreach (var item in enumerable)
            {
                if (item is TResult result)
                {
                    yield return result;
                }
            }
        }

        #endregion

        #region Join

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Join<T>(this IEnumerable<T> enumerable, T sep)
        {
            foreach (var (i, item) in enumerable.Enumerate())
            {
                if (i > 0)
                {
                    yield return sep;
                }

                yield return item;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Join(this IEnumerable<float> enumerable, float sep)
        {
            return enumerable.Join<float>(sep).Sum();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Join(this IEnumerable<int> enumerable, int sep)
        {
            return enumerable.Join<int>(sep).Sum();
        }

        #endregion

        #region Add

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<T>(this ICollection<T> collection, [CanBeNull] T item, int count)
        {
            count.DoActionNTimes(() => { collection.Add(item); });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        #endregion

        #region Min & Max



        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static TResult MinOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
        //{
        //    var list = enumerable.ToList();
        //    return list.Count == 0 ? default : list.Min(selector);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T MinOrDefault<T>(this IEnumerable<T> enumerable, object selector)
        //{
        //    return enumerable.MinOrDefault(item => item);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static TResult MaxOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
        //{
        //    var list = enumerable.ToList();
        //    return list.Count == 0 ? default : list.Max(selector);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static T MaxOrDefault<T>(this IEnumerable<T> enumerable)
        //{
        //    return enumerable.MaxOrDefault(item => item);
        //}

        #endregion

        #region Where

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable.Where(item => item != null);
        }

        #endregion

        #region Zip

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> enumerableOne, IEnumerable<T2> enumerableTwo)
        {
            return enumerableOne.Zip(enumerableTwo, (t1, t2) => (t1, t2));
        }

        #endregion

        #region Sort

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(this List<T> list, Func<T, int> weightGetter)
        {
            list.Sort((item1, item2) => weightGetter(item1).CompareTo(weightGetter(item2)));
        }

        #endregion

        #region Count

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this IEnumerable<T> enumerable, T itemToCount)
        {
            return enumerable.Count(item => item.Equals(itemToCount));
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(T, T)> Adjacent<T>(this IEnumerable<T> enumerable, bool includingBound = false)
        {
            var list = enumerable.ToList();
            var first = list.First();

            if (includingBound == true)
            {
                yield return (default, first);
            }

            foreach (var item in list.Skip(1))
            {
                yield return (first, item);
                first = item;
            }

            if (includingBound == true)
            {
                yield return (first, default);
            }
        }

        #endregion

        #region Check

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        #endregion

        public static (T, T) Between<T, TTarget>(this TTarget target, IEnumerable<T> enumerable, Func<T, TTarget> getBound)
            where TTarget : IComparable
            where T : class
        {

            foreach (var (first, second) in enumerable.Adjacent(true))
            {
                if (first == null)
                {
                    if (target.Below(getBound(second)))
                    {
                        return (null, second);
                    }
                }
                else if (second == null)
                {
                    if (target.Above(getBound(first)))
                    {
                        return (first, null);
                    }
                }
                else if (target.BetweenInclusive(getBound(first), getBound(second)))
                {
                    return (first, second);
                }
            }

            return (null, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(T, int)> GroupBy<T>(this IEnumerable<T> array, IEnumerable<int> allGroupLength)
        {
            var temp = array.ToArray();
            int index = 0, groupIndex = 0;
            foreach (var groupLength in allGroupLength)
            {
                if (index >= temp.Length)
                {
                    yield break;
                }
                for (int i = index; i < index + groupLength && i < temp.Length; i++)
                {
                    yield return (temp[i], groupIndex);
                }
                index += groupLength;
                groupIndex++;
            }
        }

        public static IEnumerable<T> CircularlyRepeat<T>(this IEnumerable<T> array, int length, int repeatStart = 0)
        {
            if (length <= 0)
            {
                yield break;
            }

            var enumerable = array as T[] ?? array.ToArray();

            if (enumerable.Length <= 0)
            {
                yield break;
            }

            repeatStart = repeatStart.Clamp(0, enumerable.Length - 1);

            int times = (length - repeatStart) / (enumerable.Length - repeatStart);
            int extraLength = (length - repeatStart) % (enumerable.Length - repeatStart);

            for (int i = 0; i < repeatStart; i++)
            {
                yield return enumerable[i];
            }

            for (int t = 0; t < times; t++)
            {
                for (int i = repeatStart; i < enumerable.Length; i++)
                {
                    yield return enumerable[i];
                }
            }

            for (int i = 0; i < extraLength; i++)
            {
                yield return enumerable[repeatStart + i];
            }
        }

        public static IEnumerable<T> PingPongRepeat<T>(this IEnumerable<T> enumerable, int length, int repeatStart = 0)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();

            if (array.Length == 0 || length <= 0)
            {
                yield break;
            }

            repeatStart = repeatStart.Clamp(array.Length);

            if (length > repeatStart)
            {
                for (int i = 0; i < repeatStart; i++)
                {
                    yield return array[i];
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    yield return array[i];
                }
                yield break;
            }

            int pingPongLength = length - repeatStart;
            bool forward = true;
            int index = repeatStart;
            int count = 0;
            while (count < pingPongLength)
            {
                if (index < 0 || index >= array.Length)
                {
                    Note.note.Warning(index, array.Length, forward);
                }
                yield return array[index];
                count++;

                if (forward)
                {
                    index++;
                    if (index >= array.Length)
                    {
                        index = array.Length - 2;
                        forward = false;
                    }
                }
                else
                {
                    if (index <= repeatStart)
                    {
                        index++;
                        forward = true;
                    }
                    else
                    {
                        index--;
                    }


                }
            }
        }

        public static void Rotate<T>(this List<T> list, int offset)
        {
            if (list is not { Count: > 1 })
            {
                return;
            }

            offset = offset.Modulo(list.Count);

            var temp = new List<T>(list.GetRange(list.Count - offset, offset));
            temp.AddRange(list.GetRange(0, list.Count - offset));
            list.Clear();
            list.AddRange(temp);
        }

        //#region Deprecated

        //public static List<T> Copy<T>(this List<T> origin)
        //{
        //    List<T> newList = new();

        //    newList.AddRange(origin);

        //    return newList;
        //}

        //public static void InitArray<T>(ref T[][] array, int width, int height)
        //{
        //    if (width <= 0 || height <= 0)
        //    {
        //        Note.note.Error("InitArray的width和height参数必须大于0");
        //    }

        //    array = new T[width][];

        //    for (int i = 0; i < width; i++)
        //    {
        //        array[i] = new T[height];
        //    }
        //}

        //public static void InitArray<T>(ref T[][][] array, int width, int height, int length)
        //{
        //    if (width <= 0 || height <= 0 || length <= 0)
        //    {
        //        Note.note.Error("InitArray的width,height和length参数必须大于0");
        //    }

        //    array = new T[width][][];

        //    for (int i = 0; i < width; i++)
        //    {
        //        InitArray(ref array[i], height, length);
        //    }
        //}

        //public static void InitArray<T>(ref T[][][] array, Vector3Int size)
        //{
        //    InitArray(ref array, size.x, size.y, size.z);
        //}

        ////public static bool CheckArraySize<T>(ref T[][] array, int width, int height)
        ////{
        ////    if (array == null)
        ////    {
        ////        return false;
        ////    }
        ////}

        //public static Vector2Int GetArraySize<T>(this T[][] array)
        //{
        //    if (array == null)
        //    {
        //        return new(0, 0);
        //    }

        //    Vector2Int result = new(0, 0);

        //    if (array.Length == 0)
        //    {
        //        return result;
        //    }

        //    result.x = array.Length;

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        if (array[i] == null && array[i].Length > result.y)
        //        {
        //            result.y = array[i].Length;
        //        }
        //    }

        //    return result;
        //}

        //public static Vector3Int GetArraySize<T>(this T[][][] array)
        //{
        //    if (array == null)
        //    {
        //        return new(0, 0, 0);
        //    }

        //    Vector3Int result = new(0, 0, 0);

        //    if (array.Length == 0)
        //    {
        //        return result;
        //    }

        //    result.x = array.Length;

        //    foreach (var subarray in array)
        //    {
        //        Vector2Int subsize = GetArraySize(subarray);

        //        if (subsize.x > result.y)
        //        {
        //            result.y = subsize.x;
        //        }
        //        if (subsize.y > result.z)
        //        {
        //            result.z = subsize.y;
        //        }
        //    }

        //    return result;
        //}

        //#endregion
    }
}

