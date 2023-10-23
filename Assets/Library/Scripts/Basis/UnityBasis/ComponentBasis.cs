using Basis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ComponentFunc
{
    public static T QueryComponentOnChildren<T>(this Transform t) where T : Component
    {
        foreach (var transform in t.GetAllChildren(true))
        {
            var component = transform.GetComponent<T>();

            if (component != null)
            {
                return component;
            }
        }

        return default;
    }

    public static T QueryComponentOnChildren<T>(this Transform t, string name) where T : Component
    {
        foreach (var transform in t.GetAllChildren(true))
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

    public static T QueryComponentOnParents<T>(this Transform t) where T : Component
    {
        foreach (var transform in t.GetAllParents(true))
        {
            var component = transform.GetComponent<T>();

            if (component != null)
            {
                return component;
            }
        }

        return default;
    }
}
