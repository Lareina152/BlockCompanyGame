using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace Basis
{
    public enum UpdateType
    {
        FixedUpdate,
        Update,
        LateUpdate,
        OnGUI,
    }

    public static class TransformBasis
    {
        #region Children Operation Ext

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearChildren(this Transform t)
        {
            foreach (var child in t.GetChildren())
            {
                if (child.childCount > 0)
                {
                    ClearChildren(child);
                }

                Object.DestroyImmediate(child.gameObject);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable GetAllChildrenNames(this Transform t, bool includingSelf)
        {
            return GetAllChildren(t, includingSelf).Select(transform => transform.name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable GetAllChildrenNames<T>(this Transform t, bool includingSelf) where T : Component
        {
            return GetAllChildren(t, includingSelf).Where(transform => transform.GetComponent<T>() != null).
                Select(transform => transform.name);
        }

        #endregion

        #region Universal Tree

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Transform> GetChildren(this Transform t)
        {
            List<Transform> children = new();
            for (int i = 0; i < t.childCount; i++)
            {
                children.Add(t.GetChild(i));
            }
            return children;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Transform> GetAllChildren(this Transform t, bool includingSelf)
        {
            return t.GetAllChildren(includingSelf, transform => transform.GetChildren());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<Transform> GetAllParents(this Transform t, bool includingSelf)
        {
            return t.GetAllParents(includingSelf, transform => transform.parent);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetGlobalArguments(this Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetLocalArguments(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LocalFlipX(this Transform t)
        {
            t.localScale = t.localScale.ReplaceX(-t.localScale.x);
            t.localPosition = t.localPosition.ReplaceX(-t.localPosition.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LocalFlipX(this Transform t, bool flipState)
        {
            if (t.localScale.x > 0 && flipState == true)
            {
                t.LocalFlipX();
            }

            if (t.localScale.x < 0 && flipState == false)
            {
                t.LocalFlipX();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EuclideanDistance(this Transform t1, Transform t2)
        {
            return t1.position.EuclideanDistance(t2.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAngle(this Transform t, float angle)
        {
            t.eulerAngles = new(0, 0, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLocalAngle(this Transform t, float angle)
        {
            t.localEulerAngles = new(0, 0, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAngle(this Transform t, float angle)
        {
            t.eulerAngles += new Vector3(0, 0, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddLocalAngle(this Transform t, float angle)
        {
            t.localEulerAngles += new Vector3(0, 0, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDirection(this Transform t, Vector2 dir)
        {
            float angle = -Vector2.SignedAngle(dir, Vector2.up);

            t.eulerAngles = new(0, 0, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLerpDirection(this Transform t, Vector2 dir, float lerp, float threshold = 1f)
        {
            float angle = Vector2.SignedAngle(Vector2.up, dir);

            if (t.eulerAngles.z.Distance(angle) < threshold)
            {
                SetAngle(t, angle);
                return;
            }

            t.rotation = t.rotation.Lerp(Quaternion.Euler(new(0, 0, angle)), lerp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveTo(this Transform t, Vector3 targetPos, float speed)
        {
            t.position += t.LinearSpeed(targetPos, speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveTo(this Transform t, Component target, float speed)
        {
            t.MoveTo(target.transform.position, speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveToDir(this Transform t, Vector3 dir)
        {
            t.position += dir;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LerpTo(this Transform t, Vector3 targetPos, float speed)
        {
            t.position = Vector3.Lerp(t.position, targetPos, speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LerpTo(this Transform t, Transform target, float speed)
        {
            t.LerpTo(target.position, speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LerpSpeed(this Transform t, Vector3 targetPos, float speed)
        {
            return Vector3.Lerp(t.position, targetPos, speed) - t.position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LinearSpeed(this Transform t, Vector3 targetPos, float speed)
        {
            return (targetPos - t.position).normalized * speed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPositionOnCircle(this Vector2 center, float radius, float angle)
        {
            float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = center.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            return new Vector2(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPositionOnCircle(this Transform center, float radius, float angle)
        {
            return center.position.To2D().GetPositionOnCircle(radius, angle);
        }
    }
}

