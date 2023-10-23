using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Basis
{
    #region Pool

    public interface IGameObjectPool<T> where T : Component
    {
        T Get(Func<T> onCreate);
        T Get(T prefab, Transform parent = null);
        void Return(T item);
        public bool Contains(T item);
    }

    [Serializable]
    public class GameObjectLimitPool<T> where T : Component
    {
        [ShowInInspector]
        private readonly Stack<T> objects = new();

        [ShowInInspector, DisplayAsString]
        private readonly int maxCapacity;

        public GameObjectLimitPool(int maxCapacity)
        {
            this.maxCapacity = maxCapacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get()
        {
            if (objects.Count > 0)
            {
                var newOne = objects.Pop();
                newOne.gameObject.SetActive(true);
                return newOne;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(T item)
        {
            if (objects.Count >= maxCapacity)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
            else
            {
                item.gameObject.SetActive(false);
                objects.Push(item);
            }
        }

        public int Count => objects.Count;
    }

    [Serializable]
    public class GameObjectPool<T> : IGameObjectPool<T> where T : Component
    {
        [ShowInInspector]
        private readonly Stack<T> stack = new();

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => stack.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(Func<T> onCreate)
        {
            var newOne = stack.Count > 0 ? stack.Pop() : onCreate();

            newOne.SetActive(true);
            return newOne;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(T prefab, Transform parent = null)
        {
            var newObject = Get(() =>
            {
                var newObject = Object.Instantiate(prefab, parent);

                return newObject;
            });

            if (parent != null)
            {
                newObject.transform.SetParent(parent);
                newObject.transform.SetAsLastSibling();
            }

            return newObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(T item)
        {
            item.SetActive(false);
            stack.Push(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return stack.Contains(item);
        }
    }

    [Serializable]
    public class GameObjectHashPool<T> : IGameObjectPool<T> where T : Component
    {
        [ShowInInspector]
        private readonly HashSet<T> hashSet = new();

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => hashSet.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(Func<T> onCreate)
        {
            T newOne;
            if (hashSet.Count > 0)
            {
                newOne = hashSet.First();
                hashSet.Remove(newOne);
            }
            else
            {
                newOne = onCreate();
            }

            newOne.SetActive(true);
            return newOne;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(T prefab, Transform parent = null)
        {
            var newObject = Get(() =>
            {
                var newObject = Object.Instantiate(prefab, parent);

                return newObject;
            });

            if (parent != null)
            {
                newObject.transform.SetParent(parent);
                newObject.transform.SetAsLastSibling();
            }

            return newObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(T item)
        {
            if (hashSet.Add(item))
            {
                item.SetActive(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return hashSet.Contains(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            return hashSet.Remove(item);
        }
    }

    #endregion

    #region Queue

    [Serializable]
    public class GameObjectLimitQueue<T> where T : Component
    {
        [ShowInInspector]
        private Queue<T> queue = new();

        [ShowInInspector, DisplayAsString]
        private readonly int maxCapacity;

        public GameObjectLimitQueue(int maxCapacity)
        {
            Note.note.AssertIsAbove(maxCapacity, 0, nameof(maxCapacity));
            this.maxCapacity = maxCapacity;
            queue ??= new();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Add(Func<T> onCreate)
        {
            var result = queue.Count >= maxCapacity ? queue.Dequeue() : onCreate();

            result.transform.SetAsLastSibling();

            queue.Enqueue(result);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Add(T prefab, Transform parent = null)
        {
            var newObject = Add(() =>
            {
                var newObject = Object.Instantiate(prefab, parent);

                return newObject;
            });

            if (parent != null)
            {
                newObject.transform.SetParent(parent);
                newObject.transform.SetAsLastSibling();
            }

            return newObject;
        }
    }

    #endregion
}
