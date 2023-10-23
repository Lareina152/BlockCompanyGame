#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Basis
{
    public static class AssetBasis
    {
        public static TAsset FindAssetOfName<TAsset>(this string assetName) where TAsset : Object
        {
            return assetName.FindAssetOfName(typeof(TAsset)) as TAsset;
        }

        public static Object FindAssetOfName(this string assetName, Type type)
        {
            return assetName.FindAssetsOfName(type).FirstOrDefault();
        }

        public static IEnumerable<Object> FindAssetsOfName(this string assetName, Type type)
        {
            var assetsOfType = type.FindAssetsOfType();

            int resultCount = 0;

            foreach (var asset in assetsOfType)
            {
                if (asset.name == assetName)
                {
                    yield return asset;

                    resultCount++;
                }
            }

            if (resultCount == 0)
            {
                Debug.LogWarning($"没找到带有名称为{assetName}，type为{type}的组件的Prefab");
            }
        }

        public static IEnumerable<Object> FindAssetsOfType(this Type type)
        {
            int resultCount = 0;

            if (type.IsDerivedFrom<Component>())
            {
                var guids = AssetDatabase.FindAssets($"t:GameObject");

                if (guids.Length >= 1)
                {
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);

                        var targetObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                        if (targetObject != null && targetObject.GetComponent(type) != null)
                        {
                            yield return targetObject.GetComponent(type);

                            resultCount++;
                        }
                    }
                }
            }
            else
            {
                var guids = AssetDatabase.FindAssets($"t:{type.Name}");

                if (guids.Length >= 1)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);

                    yield return AssetDatabase.LoadAssetAtPath(path, type);

                    resultCount++;
                }
            }

            if (resultCount == 0)
            {
                if (type.IsDerivedFrom<Component>())
                {
                    Debug.LogWarning($"没找到带有type为{type}的组件的Prefab");
                }
                else
                {
                    Debug.LogWarning($"没找到type为{type}的Asset");
                }
            }
        }

        public static Object FindAssetOfType(this Type type)
        {
            return type.FindAssetsOfType().FirstOrDefault();
        }

        #region ScriptableObject

        public static ScriptableObject FindOrCreateScriptableObject(this Type type, 
            string newPath, string newName)
        {
            if (type.IsDerivedFrom<ScriptableObject>() == false)
            {
                Debug.LogWarning($"{type}不是{nameof(ScriptableObject)}的子类");
                return default;
            }

            newPath.CreateDirectory();

            var result = type.FindAssetOfType() as ScriptableObject;

            if (result == null)
            {
                var temp = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(temp, Path.Combine(newPath, $"{newName}.asset"));
                AssetDatabase.Refresh();
                //DestroyImmediate(temp, true);

                result = type.FindAssetOfType() as ScriptableObject;

                if (result == null)
                {
                    Note.note.Warning($"种类为:{type}" +
                                 $"的{nameof(ScriptableObject)}在{newPath}/{newName}.asset下创建失败");
                }
            }

            return result;
        }

        public static T FindOrCreateScriptableObjectAtPath<T>(this string path)
            where T : ScriptableObject
        {
            var result = AssetDatabase.LoadAssetAtPath<T>(path);

            if (result == null)
            {
                result = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(result, path);
                AssetDatabase.Refresh();
            }

            if (result == null)
            {
                Note.note.Warning($"种类为:{nameof(T)}" +
                             $"的资源在{path}下创建失败");
            }

            return result;
        }

        #endregion
    }
}
#endif
