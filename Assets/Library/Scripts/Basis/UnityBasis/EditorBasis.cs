#if UNITY_EDITOR


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using UnityEditor;
using UnityEngine;

public static class EditorFunc
{
    public static MonoScript MonoScriptFromType(Type targetType)
    {
        if (targetType == null) return null;
        var typeName = targetType.Name;
        if (targetType.IsGenericType)
        {
            targetType = targetType.GetGenericTypeDefinition();
            typeName = typeName[..typeName.IndexOf('`')];
        }
        return AssetDatabase.FindAssets($"{typeName} t:MonoScript")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
            .FirstOrDefault(m => m != null && m.GetClass() == targetType);
    }

    public static bool OpenScriptOfType(this Type type)
    {
        var mono = MonoScriptFromType(type);
        if (mono != null)
        {
            AssetDatabase.OpenAsset(mono);
            return true;
        }

        Note.note.Log(
            $"打开Class:{type}失败，因为同名脚本文件不存在");
        return false;
    }
}


#endif
