using Basis;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

namespace Basis
{
    public static class RectTransformBasis
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform RT(this GameObject gameObject) => gameObject.GetComponent<RectTransform>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform RT(this Component component) => component.GetComponent<RectTransform>();

        #region Size

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSize(this RectTransform rt, Vector2 size)
        {
            rt.SetSize(size.x, size.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSize(this RectTransform rt, float width, float height)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWidth(this RectTransform rt, float width)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHeight(this RectTransform rt, float height)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform CreateEmptyRectTransformObject(this string name, Transform parent)
        {
            var newObject = 
                new GameObject(name, typeof(RectTransform)).RT();
            newObject.SetParent(parent);
            newObject.ResetArguments();

            return newObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<RectTransform> GetChildren(this RectTransform rt)
        {
            return from object child in rt where child != null select child as RectTransform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetArguments(this RectTransform rt)
        {
            rt.anchoredPosition3D = Vector3.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAnchor(this RectTransform rt, Vector2 anchor)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRealPositionOnScreen(this GameObject target, Vector2 tracingOffset, Camera camera)
        {
            return camera.WorldToScreenPoint(target.transform.position) + tracingOffset.To3D();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRealPositionOnScreen(this GameObject target, Vector2 tracingOffset)
        {
            return Camera.main.WorldToScreenPoint(target.transform.position) + tracingOffset.To3D();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToScreenPoint(this Vector2 worldPoint, Camera camera)
        {
            return camera.WorldToScreenPoint(worldPoint);
        }
    }
}
