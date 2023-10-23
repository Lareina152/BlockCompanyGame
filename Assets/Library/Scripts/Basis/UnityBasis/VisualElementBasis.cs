using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Basis;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementFunc
{
    public static IEnumerable<T> GetAll<T>(this VisualElement root) where T : VisualElement
    {
        var results = new List<T>();

        if (root == null)
        {
            Note.note.Error("root不能为Null");
            return results;
        }

        root.PreorderTraverse(element =>
        {
            if (element is T targetVisualElement)
            {
                results.Add(targetVisualElement);
            }
        }, true, element => element.Children());

        return results;
    }

    public static IEnumerable<string> GetAllNames(this VisualElement root)
    {
        return root.GetAll<VisualElement>().Select(visualElement => visualElement.name);
    }

    public static IEnumerable<T> GetAll<T>(this VisualTreeAsset treeAsset) where T : VisualElement
    {
        if (treeAsset == null)
        {
            return Enumerable.Empty<T>();
        }
        return treeAsset.CloneTree().contentContainer.GetAll<T>();
    }

    public static IEnumerable<string> GetAllNames(this VisualTreeAsset treeAsset)
    {
        if (treeAsset == null)
        {
            return Enumerable.Empty<string>();
        }
        return treeAsset.GetAll<VisualElement>().Where(visualElement => visualElement.name.IsNullOrEmptyAfterTrim() == false).
            Select(visualElement => visualElement.name);
    }

    public static IEnumerable<string> GetAllNames<T>(this VisualTreeAsset treeAsset) where T : VisualElement
    {
        if (treeAsset == null)
        {
            return Enumerable.Empty<string>();
        }
        return treeAsset.GetAll<T>().Where(visualElement => visualElement.name.IsNullOrEmptyAfterTrim() == false).
            Select(visualElement => visualElement.name);
    }
}
