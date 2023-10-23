using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Basis {
    public static class GameObjectFunc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive(this Component component, bool value)
        {
            component.gameObject.SetActive(value);
        }

        #region Component Basic Operations

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this Object unityObject) where T : Component
        {
            return unityObject switch
            {
                GameObject => unityObject.ConvertTo<GameObject>().GetComponent<T>(),
                Component => unityObject.ConvertTo<Component>().GetComponent<T>(),
                _ => null
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }

        #region GetOrAdd

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            var result = component.GetComponent<T>();

            if (result == null)
            {
                result = component.gameObject.AddComponent<T>();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static Component GetOrAddComponent(this Component component, Type componentType)
        {
            var result = component.GetComponent(componentType);

            if (result == null)
            {
                result = component.gameObject.AddComponent(componentType);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var result = gameObject.GetComponent<T>();

            if (result == null)
            {
                result = gameObject.AddComponent<T>();
            }

            return result;
        }

        #endregion

        #region Remove

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
        {
            var target = gameObject.GetComponent<T>();

            if (target != null)
            {
                Object.Destroy(target);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent<T>(this Component component) where T : Component
        {
            RemoveComponent<T>(component.gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponents<T>(this GameObject gameObject) where T : Component
        {
            var targets = gameObject.GetComponents<T>();

            foreach (var target in targets)
            {
                Object.Destroy(target);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponents<T>(this Component component) where T : Component
        {
            RemoveComponents<T>(component.gameObject);
        }

        #endregion

        #endregion

        #region FindParent

        public static GameObject FindFirstParentGameObject(this GameObject gameObject, Func<GameObject, bool> isObjectFunc, bool includingItself = true)
        {
            if (includingItself == true)
            {
                if (isObjectFunc(gameObject))
                {
                    return gameObject;
                }
                else
                {
                    if (gameObject.transform.parent == default(Transform))
                    {
                        return default(GameObject);
                    }
                    else
                    {
                        return FindFirstParentGameObject(gameObject.transform.parent.gameObject, isObjectFunc, true);
                    }
                }
            }
            else
            {
                if (gameObject.transform.parent == default(Transform))
                {
                    return default(GameObject);
                }
                else
                {
                    return FindFirstParentGameObject(gameObject.transform.parent.gameObject, isObjectFunc, true);
                }
            }
        }

        public static T FindFirstParentComponent<T>(this GameObject gameObject, bool includingItself = true) where T : Component
        {
            GameObject result = FindFirstParentGameObject(gameObject, (GameObject parent) => {
                if (parent.GetComponent<T>() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if (result != default(GameObject))
            {
                return result.GetComponent<T>();
            }
            else
            {
                return default(T);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FindFirstParentComponent<T>(this Component component, bool includingItself = true) where T : Component
        {
            return FindFirstParentComponent<T>(component.gameObject, includingItself);
        }

        public static List<GameObject> FindParentsGameObjects(this GameObject gameObject, Func<GameObject, bool> isObjectFunc, bool includingItself = true)
        {
            List<GameObject> result = new List<GameObject>();

            if (includingItself == true)
            {
                if (isObjectFunc(gameObject))
                {
                    result.Add(gameObject);
                }
            }

            if (gameObject.transform.parent != default(Transform))
            {
                result.AddRange(FindParentsGameObjects(gameObject.transform.parent.gameObject, isObjectFunc, true));
            }

            return result;
        }

        public static List<T> FindParentsComponents<T>(this GameObject gameObject, bool includingItself = false) where T : Component
        {
            List<GameObject> resultObjects = FindParentsGameObjects(gameObject, (GameObject objectToJudge) => {
                if (objectToJudge.GetComponent<T>() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }, includingItself);

            List<T> result = new List<T>();

            foreach (GameObject resultObject in resultObjects)
            {
                if (resultObject.GetComponent<T>() != null)
                {
                    result.Add(resultObject.GetComponent<T>());
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> FindParentsComponents<T>(this Component component, bool includingItself = false) where T : Component
        {
            return FindParentsComponents<T>(component.gameObject, includingItself);
        }

        public static GameObject FindFirstChildGameObject(this GameObject gameObject, Func<GameObject, bool> isObjectFunc, bool includingItself = false)
        {
            if (includingItself == true)
            {
                if (isObjectFunc(gameObject) == true)
                {
                    return gameObject;
                }
            }

            foreach (Transform child in gameObject.transform)
            {
                GameObject childResult = FindFirstChildGameObject(child.gameObject, isObjectFunc, true);
                if (childResult != default(GameObject))
                {
                    return childResult;
                }
            }

            return default(GameObject);
        }

        #endregion

        #region FindChild

        public static T FindFirstChildComponent<T>(this GameObject gameObject, bool includingItself = false) where T : Component
        {
            GameObject result = FindFirstChildGameObject(gameObject, (GameObject objectToJudge) => {
                if (objectToJudge.GetComponent<T>() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }, includingItself);

            if (result != default(GameObject))
            {
                return result.GetComponent<T>();
            }
            else
            {
                return default(T);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FindFirstChildComponent<T>(this Component component, bool includingItself = false) where T : Component
        {
            return FindFirstChildComponent<T>(component.gameObject, includingItself);
        }

        public static List<GameObject> FindChildrenGameObjects(this GameObject gameObject, Func<GameObject, bool> isObjectFunc, bool includingItself = false)
        {
            List<GameObject> result = new List<GameObject>();

            if (includingItself == true)
            {
                if (isObjectFunc(gameObject) == true)
                {
                    result.Add(gameObject);
                }
            }

            foreach (Transform child in gameObject.transform)
            {
                result.AddRange(FindChildrenGameObjects(child.gameObject, isObjectFunc, true));
            }

            return result;
        }

        public static List<T> FindChildrenComponents<T>(this GameObject gameObject, bool includingItself = false) where T : Component
        {
            List<GameObject> resultObjects = FindChildrenGameObjects(gameObject, (GameObject objectToJudge) => {
                if (objectToJudge.GetComponent<T>() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }, includingItself);

            List<T> result = new List<T>();

            foreach (GameObject resultObject in resultObjects)
            {
                if (resultObject.GetComponent<T>() != null)
                {
                    result.Add(resultObject.GetComponent<T>());
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> FindChildrenComponents<T>(this Component component, bool includingItself = false) where T : Component
        {
            return FindChildrenComponents<T>(component.gameObject, includingItself);
        }

        #endregion

        #region FindOrCreate

        #region Prefab

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static T FindOrCreatePrefab<T>(this T prefab, Transform parent = null, Action<T> afterCreate = null) where T : Component
        {
#if UNITY_EDITOR
            return FindOrCreateComponent(Object.FindObjectOfType<T>,
                () => PrefabUtility.InstantiatePrefab(prefab.gameObject).GetComponent<T>(), parent, afterCreate);
#endif
#pragma warning disable CS0162 // Unreachable code detected
            return FindOrCreateComponent(Object.FindObjectOfType<T>,
                () => Object.Instantiate(prefab.gameObject).GetComponent<T>(), parent, afterCreate);
#pragma warning restore CS0162 // Unreachable code detected
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static GameObject FindOrCreatePrefab(this string name, GameObject prefab, Transform parent = null)
        {
#if UNITY_EDITOR
            return name.FindOrCreateObject(() => (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject), parent);
#endif
#pragma warning disable CS0162 // Unreachable code detected
            return name.FindOrCreateObject(() => (GameObject)Object.Instantiate(prefab.gameObject), parent);
#pragma warning restore CS0162 // Unreachable code detected
        }

        #endregion

        #region Object

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static GameObject FindOrCreateObject(Func<GameObject> onFind, Func<GameObject> onCreate, 
            Transform parent = null, Action<GameObject> afterCreate = null)
        {
            var newObject = onFind();
            
            if (newObject == null)
            {
                newObject = onCreate();
                afterCreate?.Invoke(newObject);
            }

            if (parent != null)
            {
                newObject.transform.SetParent(parent);
            }

            return newObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static GameObject FindOrCreateObject(this string name, Func<GameObject> onCreate, Transform parent = null)
        {
            return FindOrCreateObject(() => GameObject.Find(name), onCreate, parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static GameObject FindOrCreateObject(this string name, Transform parent = null)
        {
            return name.FindOrCreateObject(() => new GameObject(name), parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static GameObject FindOrCreateObject(this string name, [NotNull] GameObject parentObject)
        {
            return name.FindOrCreateObject(parentObject.transform);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FindObject<T>(this string name) where T : Component
        {
            var results = Object.FindObjectsOfType<T>();

            return results.FirstOrDefault(result => result.name == name);
        }

        #endregion

        #region Component

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static T FindOrCreateComponent<T>(Func<T> onFind, Func<T> onCreate, Transform parent = null, 
            Action<T> afterCreate = null) 
            where T : Component
        {
            var result = onFind();
            if (result == null)
            {
                result = onCreate();
                afterCreate?.Invoke(result);
            }

            if (parent != null)
            {
                result.transform.SetParent(parent);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static T FindOrCreateComponent<T>([NotNull] this GameObject attachedObject) where T : Component
        {
            return FindOrCreateComponent(attachedObject.GetComponent<T>, attachedObject.AddComponent<T>);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static T FindOrCreateComponent<T>([NotNull] this Component attachedComponent) where T : Component
        {
            return attachedComponent.gameObject.FindOrCreateComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static T FindOrCreateUniqueComponent<T>([NotNull] this GameObject attachedObject)
            where T : Component
        {
            return FindOrCreateComponent(Object.FindObjectOfType<T>, attachedObject.FindOrCreateComponent<T>);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static T FindOrCreateUniqueComponent<T>([NotNull] this string name, Transform parent = null)
            where T : Component
        {
            return FindOrCreateComponent(() => FindObject<T>(name), 
                name.FindOrCreateObject().FindOrCreateComponent<T>,
                parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        public static T FindOrCreateUniqueComponent<T>([NotNull] this string name, [NotNull] GameObject parentObject)
            where T : Component
        {
            return name.FindOrCreateUniqueComponent<T>(parentObject.transform);
        }

        #endregion

        #region Common Create Preset

        //[MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        //public static Canvas FindOrCreateCanvas(this Transform parent)
        //{
        //    var canvas = "Canvas".FindOrCreateUniqueComponent<Canvas>(parent);


        //    var scaler = canvas.FindOrCreateComponent<CanvasScaler>();
        //    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        //    scaler.referenceResolution = new(1920, 1080);
        //    scaler.matchWidthOrHeight = 0.5f;

        //    canvas.FindOrCreateComponent<GraphicRaycaster>();
        //    if (canvas.renderMode == RenderMode.WorldSpace)
        //    {
        //        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //    }

        //    return canvas;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining), NotNull]
        //public static Canvas CreateCanvas(this Transform parent)
        //{
        //    var canvas = new GameObject("Canvas", 
        //        typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
        //    canvas.transform.SetParent(parent);

        //    var scaler = canvas.GetComponent<CanvasScaler>();
        //    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        //    scaler.referenceResolution = new(1920, 1080);
        //    scaler.matchWidthOrHeight = 0.5f;

        //    canvas.GetComponent<GraphicRaycaster>();
        //    if (canvas.renderMode == RenderMode.WorldSpace)
        //    {
        //        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //    }

        //    return canvas;
        //}

        #endregion


        #endregion


    }
}
