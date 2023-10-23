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
    [LabelText("ѡ�е���Դ")]
    [PropertyOrder(1000)]
    public Object[] selectedObjects => Selection.objects;

    [Button("�»���������������", ButtonSizes.Medium)]
    private static void RenameAssetToSnakeCase()
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.ToSnakeCase());
        }
    }

    [Button("��˹��������������", ButtonSizes.Medium)]
    private static void RenameAssetToPascalCase()
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.ToPascalCase(" "));
        }
    }

    [Button("�滻", ButtonSizes.Medium, Style = ButtonStyle.Box)]
    private static void NameReplace([LabelText("���ַ���")] string oldString, [LabelText("���ַ���")] string newString)
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.Replace(oldString, newString));
        }
    }

    [Button("���ȫ���ո�", ButtonSizes.Medium)]
    private static void ClearAllSpaces()
    {
        foreach (var o in Selection.objects)
        {
            string selectedAssetPath = AssetDatabase.GetAssetPath(o);
            AssetDatabase.RenameAsset(selectedAssetPath, o.name.Replace(" ", ""));
        }
    }

    [Button("�����β�ո�", ButtonSizes.Medium)]
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