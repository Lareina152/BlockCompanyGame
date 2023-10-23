#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class NamingTools
{
    [ShowInInspector]
    [LabelText("选中的资源")]
    [PropertyOrder(1000)]
    public Object[] selectedObjects => Selection.objects;

    [Button("下划线命名法重命名", ButtonSizes.Medium)]
    private static void RenameAssetToSnakeCase()
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.ToSnakeCase());
        }
    }

    [Button("帕斯卡命名法重命名", ButtonSizes.Medium)]
    private static void RenameAssetToPascalCase()
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.ToPascalCase(" "));
        }
    }

    [Button("替换", ButtonSizes.Medium, Style = ButtonStyle.Box)]
    private static void NameReplace([LabelText("旧字符串")] string oldString, [LabelText("新字符串")] string newString)
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.Replace(oldString, newString));
        }
    }

    [Button("清除全部空格", ButtonSizes.Medium)]
    private static void ClearAllSpaces()
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.Replace(" ", ""));
        }
    }

    [Button("清除首尾空格", ButtonSizes.Medium)]
    private static void StripSpaces()
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.Trim());
        }
    }
}

#endif