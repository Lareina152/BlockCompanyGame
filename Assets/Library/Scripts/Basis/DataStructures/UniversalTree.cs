using Basis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Basis
{
    public interface IParentOnlyNode<out T> where T : class, IParentOnlyNode<T>
    {
        public T GetParent();
    }

    public interface IChildrenOnlyNode<out T> where T : class, IChildrenOnlyNode<T>
    {
        public IEnumerable<T> GetChildren();
    }

    public interface IUniversalTree<T> : IParentOnlyNode<T>, IChildrenOnlyNode<T> 
        where T : class, IUniversalTree<T>, IChildrenOnlyNode<T>
    {
        public bool DirectEquals(T other);
    }

    public static class UniversalTree
    {
        public static T GetRoot<T>(this T node) where T : class, IParentOnlyNode<T>
        {
            var result = node;

            node.TraverseToRoot(parent => result = parent, false);

            return result;
        }

        #region Traverse To Root

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TraverseToRoot<T>(this T node, Func<T, bool> willBreak, bool includingSelf)
            where T : class, IParentOnlyNode<T>
        {
            TraverseToRoot(node, willBreak, includingSelf, node => node.GetParent());
        }

        public static void TraverseToRoot<T>(this T node, Func<T, bool> willBreak, bool includingSelf,
            Func<T, T> parentGetter)
            where T : class
        {
            while (node != null)
            {
                if (includingSelf && willBreak(node))
                {
                    return;
                }

                node = parentGetter(node);
                includingSelf = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TraverseToRoot<T>(this T node, Action<T> action, bool includingSelf)
            where T : class, IParentOnlyNode<T>
        {
            TraverseToRoot(node, action, includingSelf, node => node.GetParent());
        }

        public static void TraverseToRoot<T>(this T node, Action<T> action, bool includingSelf, Func<T, T> parentGetter)
            where T : class
        {
            while (node != null)
            {
                if (includingSelf)
                {
                    action(node);
                }

                node = parentGetter(node);
                includingSelf = true;
            }
        }

        #endregion

        #region Parent

        public static bool HasParent<T>(this T node, T parent, bool includingSelf)
            where T : class, IUniversalTree<T>
        {
            bool hasParent = false;

            node.TraverseToRoot(parentToCheck =>
            {
                if (parentToCheck.DirectEquals(parent))
                {
                    hasParent = true;
                    return true;
                }

                return false;
            }, includingSelf);

            return hasParent;
        }

        public static bool HasParent<T, TParentCheck>(this T node, TParentCheck parent, Func<T, TParentCheck, bool> checkFunc, bool includingSelf)
            where T : class, IUniversalTree<T>
        {
            if (parent == null)
            {
                Note.note.Error("parent为Null");
            }

            bool hasParent = false;

            node.TraverseToRoot(parentToCheck =>
            {
                if (checkFunc(parentToCheck, parent))
                {
                    hasParent = true;
                    return true;
                }

                return false;
            }, includingSelf);

            return hasParent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<T> GetAllParents<T>(this T node, bool includingSelf)
            where T : class, IParentOnlyNode<T>
        {
            return GetAllParents(node, includingSelf, node => node.GetParent());
        }

        public static IReadOnlyList<T> GetAllParents<T>(this T node, bool includingSelf, Func<T, T> parentGetter)
            where T : class
        {
            var result = new List<T>();

            node.TraverseToRoot(parent => result.Add(parent), includingSelf, parentGetter);

            return result;
        }

        #endregion

        #region Children

        public static T GetRandomDirectChildren<T>(this T node)
            where T : class, IChildrenOnlyNode<T>
            => GetRandomDirectChildren(node, node => node.GetChildren());

        public static T GetRandomDirectChildren<T>(this T node, Func<T, IEnumerable<T>> childrenGetter)
            => childrenGetter(node).Choose();

        public static IEnumerable<T> GetAllLeaves<T>(this T node, bool includingSelf)
            where T : class, IChildrenOnlyNode<T>
        {
            return GetAllLeaves(node, includingSelf, node => node.GetChildren());
        }

        public static IEnumerable<T> GetAllLeaves<T>(this T node, bool includingSelf,
            Func<T, IEnumerable<T>> childrenGetter)
        {
            return node.GetAllChildren(includingSelf, childrenGetter).Where(child => !childrenGetter(child).Any());
        }

        public static IEnumerable<T> GetAllChildren<T>(this T node, bool includingSelf)
            where T : class, IChildrenOnlyNode<T>
        {
            return GetAllChildren(node, includingSelf, node => node.GetChildren());
        }

        public static IEnumerable<T> GetAllChildren<T>(this T node, bool includingSelf, 
            Func<T, IEnumerable<T>> childrenGetter)
        {
            var result = new List<T>();

            node.PreorderTraverse(child =>
            {
                result.Add(child);
                return false;
            }, includingSelf, childrenGetter);

            return result;
        }

        public static IEnumerable<T> FindChildren<T>(this T node, bool includingSelf, Func<T, bool> predictor)
            where T : class, IChildrenOnlyNode<T>
        {
            return FindChildren(node, includingSelf, node => node.GetChildren(), predictor);
        }

        public static IEnumerable<T> FindChildren<T>(this T node, bool includingSelf,
            Func<T, IEnumerable<T>> childrenGetter,
            Func<T, bool> predicate)
        {
            return GetAllChildren(node, includingSelf, childrenGetter).Where(predicate);
        }

        public static T FindChild<T>(this T node, bool includingSelf, Func<T, bool> predictor)
            where T : class, IChildrenOnlyNode<T>
        {
            return FindChild(node, includingSelf, node => node.GetChildren(), predictor);
        }

        public static T FindChild<T>(this T node, bool includingSelf,
            Func<T, IEnumerable<T>> childrenGetter,
            Func<T, bool> predicate)
        {
            return GetAllChildren(node, includingSelf, childrenGetter).FirstOrDefault(predicate);
        }

        public static bool TryFindChild<T>(this T node, bool includingSelf, Func<T, bool> predictor, out T result)
            where T : class, IChildrenOnlyNode<T>
        {
            return TryFindChild(node, includingSelf, node => node.GetChildren(), predictor, out result);
        }

        public static bool TryFindChild<T>(this T node, bool includingSelf,
            Func<T, IEnumerable<T>> childrenGetter,
            Func<T, bool> predicate, out T result)
        {
            var results = FindChildren(node, includingSelf, childrenGetter, predicate).ToList();
            if (results.Count == 0)
            {
                result = default;
                return false;
            }

            result = results[0];
            return true;
        }

        public static bool HasChild<T>(this T node, bool includingSelf, Func<T, bool> predicate)
            where T : class, IChildrenOnlyNode<T>
        {
            return HasChild(node, includingSelf, node => node.GetChildren(), predicate);
        }

        public static bool HasChild<T>(this T node, bool includingSelf, Func<T, IEnumerable<T>> childrenGetter,
            Func<T, bool> predicate)
        {
            return FindChildren(node, includingSelf, childrenGetter, predicate).Count() != 0;
        }

        public static void PreorderTraverse<T>(this T node, Action<T> action, bool includingSelf)
            where T : class, IChildrenOnlyNode<T>
        {
            PreorderTraverse(node, action, includingSelf, node => node.GetChildren());
        }

        public static void PreorderTraverse<T>(this T node, Action<T> action, bool includingSelf,
            Func<T, IEnumerable<T>> childrenGetter)
        {
            if (includingSelf)
            {
                action(node);
            }

            var children = childrenGetter(node);
            if (children != null)
            {
                foreach (var child in children)
                {
                    PreorderTraverse(child, action, true, childrenGetter);
                }
            }
        }

        public static void PreorderTraverse<T>(this T node, Func<T, bool> willBreak, bool includingSelf)
            where T : class, IChildrenOnlyNode<T>
        {
            PreorderTraverse(node, willBreak, includingSelf, node => node.GetChildren());
        }

        public static void PreorderTraverse<T>(this T node, Func<T, bool> willBreak, bool includingSelf,
            Func<T, IEnumerable<T>> childrenGetter)
        {
            if (includingSelf && willBreak(node))
            {
                return;
            }

            foreach (var child in childrenGetter(node))
            {
                PreorderTraverse(child, willBreak, true, childrenGetter);
            }
        }

        public static T PreorderTraverseToGet<T>(this T node, Func<T, bool> willBreak, bool includingSelf)
            where T : class, IChildrenOnlyNode<T>
        {
            return PreorderTraverseToGet(node, willBreak, includingSelf, node => node.GetChildren());
        }

        public static T PreorderTraverseToGet<T>(this T node, Func<T, bool> willBreak, bool includingSelf,
            Func<T, IEnumerable<T>> childrenGetter) where T : class
        {
            if (includingSelf && willBreak(node))
            {
                return node;
            }

            return childrenGetter(node).Select(child => 
                    PreorderTraverseToGet(child, willBreak, true, childrenGetter)).
                FirstOrDefault(result => result != null);
        }

        public static void InorderTraverse<T>(this T node, int nodeOrder, Func<T, bool> willBreak, bool includeSelf)
            where T : class, IChildrenOnlyNode<T>
        {
            InorderTraverse(node, nodeOrder, willBreak, includeSelf, node => node.GetChildren());
        }

        public static void InorderTraverse<T>(this T node, int nodeOrder, Func<T, bool> willBreak, bool includeSelf,
            Func<T, IEnumerable<T>> childrenGetter)
        {
            switch (nodeOrder)
            {
                case < 0:
                    Note.note.Error($"nodeOrder < 0， nodeOrder : {nodeOrder}");
                    break;
                case 0:
                    Note.note.Warning("nodeOrder is 0, use PreorderTraverse instead");
                    break;
            }

            foreach (var (index, child) in childrenGetter(node).Enumerate())
            {
                if (index == nodeOrder)
                {
                    if (includeSelf && willBreak(node))
                    {
                        return;
                    }
                }

                InorderTraverse(child, nodeOrder, willBreak, true, childrenGetter);
            }
        }

        public static void PostorderTraverse<T>(this T node, Action<T> action, bool includingSelf)
            where T : class, IChildrenOnlyNode<T>
        {
            PostorderTraverse(node, action, includingSelf, node => node.GetChildren());
        }

        public static void PostorderTraverse<T>(this T node, Action<T> action, bool includingSelf, Func<T, IEnumerable<T>> childrenGetter)
        {
            foreach (var child in childrenGetter(node))
            {
                PostorderTraverse(child, action, true, childrenGetter);
            }

            if (includingSelf)
            {
                action(node);
            }
        }

        #endregion
    }
}
