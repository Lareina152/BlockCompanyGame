using Basis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Basis.Utility
{
    public static class ComponentUtility
    {
        #region Query

        public static T QueryComponentInChildren<T>(this Component c, bool includingSelf) where T : Component
        {
            foreach (var transform in c.transform.GetAllChildren(includingSelf))
            {
                var component = transform.GetComponent<T>();

                if (component != null)
                {
                    return component;
                }
            }

            return default;
        }

        public static T QueryComponentInChildren<T>(this Component c, string name, bool includingSelf)
            where T : Component
        {
            foreach (var transform in c.transform.GetAllChildren(includingSelf))
            {
                if (transform.name != name)
                {
                    continue;
                }

                var component = transform.GetComponent<T>();

                if (component != null)
                {
                    return component;
                }
            }

            return default;
        }

        public static T QueryComponentInParents<T>(this Component c, bool includingSelf)
            where T : Component
        {
            foreach (var transform in c.transform.GetAllParents(includingSelf))
            {
                var component = transform.GetComponent<T>();

                if (component != null)
                {
                    return component;
                }
            }

            return default;
        }

        #endregion

        #region Find

        public static IEnumerable<T> FindComponentsInChildren<T>(this Component c, bool includingSelf)
            where T : Component
        {
            foreach (var transform in c.transform.GetAllChildren(includingSelf))
            {
                var components = transform.GetComponents<T>();

                foreach (var component in components)
                {
                    yield return component;
                }
            }
        }

        #endregion

        #region Has

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this Component c) where T : Component
        {
            return c.GetComponent<T>() != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>() != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponentInChildren<T>(this Component c, bool includingSelf) 
            where T : Component
        {
            return c.transform.GetAllChildren(includingSelf).
                Any(child => child.HasComponent<T>());
        }

        #endregion
    }
}
