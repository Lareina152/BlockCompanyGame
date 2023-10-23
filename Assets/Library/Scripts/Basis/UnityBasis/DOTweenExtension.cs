using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Basis
{
    public static class DOTweenExtension
    {
        public static void DOKill(this IEnumerable<Transform> targets)
        {
            foreach (var target in targets)
            {
                target.DOKill();
            }
        }

        public static Tween DOCircle(this Transform target, Vector3 center, float radius, float startAngle, float duration)
        {
            return DOTween.To(() => startAngle, newAngle =>
            {
                startAngle = newAngle;
                target.transform.position = center.To2D().GetPositionOnCircle(radius, newAngle);
            }, 360, duration).SetTarget(target);
        }

        public static Tween To<T>(DOGetter<T> getter, DOSetter<T> setter, T endValue, float duration)
        {
            return endValue switch
            {
                float => DOTween.To(getter.ConvertTo<DOGetter<float>>(), setter.ConvertTo<DOSetter<float>>(),
                    endValue.ConvertTo<float>(), duration),
                int => DOTween.To(getter.ConvertTo<DOGetter<int>>(), setter.ConvertTo<DOSetter<int>>(),
                    endValue.ConvertTo<int>(), duration),
                Vector2 => DOTween.To(getter.ConvertTo<DOGetter<Vector2>>(), setter.ConvertTo<DOSetter<Vector2>>(),
                    endValue.ConvertTo<Vector2>(), duration),
                Vector3 => DOTween.To(getter.ConvertTo<DOGetter<Vector3>>(), setter.ConvertTo<DOSetter<Vector3>>(),
                    endValue.ConvertTo<Vector3>(), duration),
                Vector4 => DOTween.To(getter.ConvertTo<DOGetter<Vector4>>(), setter.ConvertTo<DOSetter<Vector4>>(),
                    endValue.ConvertTo<Vector4>(), duration),
                Color => DOTween.To(getter.ConvertTo<DOGetter<Color>>(), setter.ConvertTo<DOSetter<Color>>(),
                    endValue.ConvertTo<Color>(), duration),
                _ => throw new ArgumentException()
            };
        }
    }

    public static class DotweenVisualElementExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DOKill(this VisualElement visualElement)
        {
            DOTween.Kill(visualElement);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TweenerCore<float, float, FloatOptions> DOFade(this VisualElement visualElement, float endValue, float duration)
        {
            var t = DOTween.To(() => visualElement.style.opacity.value,
                opacityValue => visualElement.style.opacity = opacityValue,
                endValue, duration);
            t.SetTarget(visualElement);
            return t;
        }
    }
}

